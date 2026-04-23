using System;
using System.Linq;
using ADTOSharp.UI;
using System.Transactions;
using ADTOSharp.Domain.Uow;
using System.Threading.Tasks;
using ADTO.DCloud.CodeRule.Dto;
using ADTOSharp.Authorization;
using System.Collections.Generic;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.CodeRule
{
    /// <summary>
    /// 编码配置相关接口
    /// </summary>
    [ADTOSharpAuthorize]
    public class CodeRuleAppService : DCloudAppServiceBase, ICodeRuleAppService
    {
        private readonly IRepository<CodeRule, Guid> _codeRuleRepository;
        private readonly IRepository<CodeRuleRecord, Guid> _codeRecordRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public CodeRuleAppService(
              IRepository<CodeRule, Guid> codeRuleRepository
            , IRepository<CodeRuleRecord, Guid> codeRecordRepository
            , IUnitOfWorkManager unitOfWorkManager
            )
        {
            _codeRuleRepository = codeRuleRepository;
            _codeRecordRepository = codeRecordRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// 添加编码配置信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateCodeRuleAsync(CreateCodeRuleDto input)
        {
            var isCodeExists = await _codeRuleRepository.GetAll().AnyAsync(p => p.RuleCode == input.RuleCode);
            if (isCodeExists)
            {
                throw new UserFriendlyException($"编码规则编号【{input.RuleCode}】已存在，请更换！");
            }
            input.InitSeq ??= 0;

            var info = ObjectMapper.Map<CodeRule>(input);

            info.SeqLength = info.SeqLength <= 0 ? 4 : info.SeqLength;

            await _codeRuleRepository.InsertAsync(info);
        }

        /// <summary>
        /// 修改编码配置资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateCodeRuleAsync(CreateCodeRuleDto input)
        {
            var isCodeExists = await _codeRuleRepository.GetAll().AnyAsync(p => p.RuleCode == input.RuleCode && p.Id != input.Id);
            if (isCodeExists)
            {
                throw new UserFriendlyException($"编码规则编号【{input.RuleCode}】已存在，请更换！");
            }
            var info = this._codeRuleRepository.Get(input.Id.Value);
            ObjectMapper.Map(input, info);

            await _codeRuleRepository.UpdateAsync(info);
        }

        /// <summary>
        /// 删除指定的编码配置资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task DeleteCodeRuleAsync(EntityDto<Guid> input)
        {
            var info = await this._codeRuleRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }

            await _codeRuleRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取编码配置分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<CodeRuleDto>> GetCodeRulePageList(PagedCodeRuleResultRequestDto input)
        {
            var query = _codeRuleRepository.GetAll()
                 .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.RuleName.Contains(input.KeyWord) || p.RuleCode == input.KeyWord);

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync(); ;

            return new PagedResultDto<CodeRuleDto>(totalCount, ObjectMapper.Map<List<CodeRuleDto>>(items));
        }

        /// <summary>
        /// 获取指定编码配置详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CodeRuleDto> GetCodeRuleByIdAsync(EntityDto<Guid> input)
        {
            var info = await _codeRuleRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var infoDto = ObjectMapper.Map<CodeRuleDto>(info);

            return infoDto;
        }

        /// <summary>
        /// 生成表单编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<GenerateCodeOutput> GenerateBusinessCodeAsync(GenerateCodeByRuleCodeInput input)
        {
            // 1. 查询规则（补充租户隔离，多租户场景必加）
            var query = _codeRuleRepository.GetAll();
            
            var codeRule = await query.FirstOrDefaultAsync(r => r.RuleCode == input.RuleCode);

            // 2. 规则校验
            if (codeRule == null)
            {
                throw new UserFriendlyException($"编码规则【{input.RuleCode}】不存在！");
            }
            if (!codeRule.IsActive)
            {
                throw new UserFriendlyException($"编码规则【{codeRule.RuleName}（{input.RuleCode}）】已禁用，无法生成编码！");
            }

            // 3. 防重校验（复用原有逻辑，补充租户隔离）
            if (input.BusinessId.HasValue)
            {
                var recordQuery = _codeRecordRepository.GetAll();
               
                var existingRecord = await recordQuery.FirstOrDefaultAsync(
                    r => r.RuleId == codeRule.Id
                    && r.BusinessId == input.BusinessId
                   );

                if (existingRecord != null)
                {
                    return new GenerateCodeOutput
                    {
                        GeneratedCode = existingRecord.GeneratedCode,
                        RuleCode = codeRule.RuleCode,
                    };
                }
            }

            int newSeq;
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Required))
            {
                // 核心新增：判断是否需要按日期重置流水号
                string currentDateStr = null;
                bool needReset = false;

                // 只有开启「按日期重置」且配置了日期格式，才判断重置
                if (codeRule.IsResetByDate && !string.IsNullOrWhiteSpace(codeRule.DateFormat))
                {
                    try
                    {
                        // 获取当前日期的格式化字符串（和编码中的日期格式一致）
                        currentDateStr = DateTime.Now.ToString(codeRule.DateFormat);
                        // 对比最后重置日期：不一致则需要重置
                        needReset = codeRule.LastResetDate != currentDateStr;
                    }
                    catch (Exception ex)
                    {
                        throw new UserFriendlyException($"规则【{input.RuleCode}】日期格式无效：{ex.Message}");
                    }
                }

                int updateCount;
                if (needReset)
                {
                    // 场景1：需要重置 → 流水号置为InitSeq，更新LastResetDate
                    updateCount = await _codeRuleRepository.GetAll()
                        .Where(r => r.Id == codeRule.Id )
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(r => r.CurrentSeq, codeRule.InitSeq) // 用初始值重置
                            .SetProperty(r => r.LastResetDate, currentDateStr)); // 记录当前日期标识
                    newSeq = codeRule.InitSeq;
                }
                else
                {
                    // 场景2：不重置 → 先判断CurrentSeq是否有效，无效则初始化（核心修复）
                    // 新增：如果CurrentSeq < InitSeq（比如0），先初始化到InitSeq，再自增
                    if (codeRule.CurrentSeq < codeRule.InitSeq)
                    {
                        // 初始化CurrentSeq为InitSeq（保证起始值正确）
                        updateCount = await _codeRuleRepository.GetAll()
                            .Where(r => r.Id == codeRule.Id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(r => r.CurrentSeq, codeRule.InitSeq));
                        newSeq = codeRule.InitSeq; // 首次生成用InitSeq
                    }
                    else
                    {
                        // 正常自增（基于InitSeq的基准）
                        updateCount = await _codeRuleRepository.GetAll()
                            .Where(r => r.Id == codeRule.Id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(r => r.CurrentSeq, r => r.CurrentSeq + 1));

                        // 获取自增后的最新序号
                        //var updatedRule = await _codeRuleRepository.GetAsync(codeRule.Id);
                        //newSeq = updatedRule.CurrentSeq;
                         newSeq = codeRule.CurrentSeq + 1;
                    }
                }

                // 校验更新是否成功
                if (updateCount == 0)
                {
                    throw new UserFriendlyException($"编码生成失败：规则【{input.RuleCode}】并发冲突，请重试！");
                }

                await uow.CompleteAsync();
            }

            // 解析规则拼接编码
            var codeSegments = new List<string>();
            // 前缀
            if (!string.IsNullOrWhiteSpace(codeRule.Prefix))
            {
                codeSegments.Add(codeRule.Prefix);
            }
            // 日期
            if (!string.IsNullOrWhiteSpace(codeRule.DateFormat))
            {
                try
                {
                    codeSegments.Add(DateTime.Now.ToString(codeRule.DateFormat));
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException($"规则【{input.RuleCode}】日期格式无效：{ex.Message}");
                }
            }
            // 流水号（补零）
            codeSegments.Add(newSeq.ToString().PadLeft(codeRule.SeqLength, '0'));
            // 后缀
            if (!string.IsNullOrWhiteSpace(codeRule.Suffix))
            {
                codeSegments.Add(codeRule.Suffix);
            }
            // 拼接
            var separator = string.IsNullOrWhiteSpace(codeRule.SegmentSeparator) ? "" : codeRule.SegmentSeparator;
            var finalCode = string.Join(separator, codeSegments);

            // 记录编码记录
            var codeRecord = new CodeRuleRecord
            {
                RuleId = codeRule.Id,
                Rule = codeRule,
                GeneratedCode = finalCode,
                BusinessId = input.BusinessId,
               
            };
            await _codeRecordRepository.InsertAsync(codeRecord);

            // 返回结果
            return new GenerateCodeOutput
            {
                GeneratedCode = finalCode,
                RuleCode = codeRule.RuleCode,
            };
        }
       
    }
}
