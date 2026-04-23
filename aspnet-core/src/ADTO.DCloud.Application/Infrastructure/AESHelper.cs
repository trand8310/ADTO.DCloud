using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Infrastructure
{
    /// <summary>
    /// 加密、解密帮助类
    /// </summary>
    public class AESHelper
    {
        //
        // 摘要:
        //     32位处理key
        //
        // 参数:
        //   key:
        //     原字节
        private static string GetAesKey(string key)
        {
            int length = key.Length;
            if (length < 32)
            {
                for (int i = 0; i < 32 - length; i++)
                {
                    key += "0";
                }

                return key;
            }

            return key.Substring(0, 32);
        }

        //
        // 摘要:
        //     使用AES加密字符串
        //
        // 参数:
        //   content:
        //     加密内容
        //
        //   key:
        //     秘钥
        //
        //   vi:
        //     偏移量
        //
        // 返回结果:
        //     Base64字符串结果
        public static string AesEncrypt(string content, string key, string vi = "1234567890000000")
        {
            byte[] bytes = Encoding.UTF8.GetBytes(GetAesKey(key));
            byte[] bytes2 = Encoding.UTF8.GetBytes(vi);
            byte[] bytes3 = Encoding.UTF8.GetBytes(content);
            SymmetricAlgorithm symmetricAlgorithm = Aes.Create();
            symmetricAlgorithm.IV = bytes2;
            symmetricAlgorithm.Key = bytes;
            symmetricAlgorithm.Mode = CipherMode.CBC;
            symmetricAlgorithm.Padding = PaddingMode.PKCS7;
            ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor();
            byte[] inArray = cryptoTransform.TransformFinalBlock(bytes3, 0, bytes3.Length);
            return Convert.ToBase64String(inArray);
        }

        //
        // 摘要:
        //     使用AES解密字符串
        //
        // 参数:
        //   content:
        //     内容
        //
        //   key:
        //     秘钥
        //
        //   vi:
        //     偏移量
        //
        // 返回结果:
        //     UTF8解密结果
        public static string AesDecrypt(string content, string key, string vi = "1234567890000000")
        {
            byte[] bytes = Encoding.UTF8.GetBytes(GetAesKey(key));
            byte[] bytes2 = Encoding.UTF8.GetBytes(vi);
            byte[] array = Convert.FromBase64String(content);
            SymmetricAlgorithm symmetricAlgorithm = Aes.Create();
            symmetricAlgorithm.IV = bytes2;
            symmetricAlgorithm.Key = bytes;
            symmetricAlgorithm.Mode = CipherMode.CBC;
            symmetricAlgorithm.Padding = PaddingMode.PKCS7;
            ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor();
            byte[] bytes3 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
            return Encoding.UTF8.GetString(bytes3);
        }
    }
}
