using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Extensions;
using ADTOSharp.Localization;
using ADTOSharp.Runtime.Validation;
using ADTOSharp.UI;
using ADTOSharp.Web.Configuration;

namespace ADTOSharp.Web.Models
{
    //TODO@Halil: I did not like constructing ErrorInfo this way. It works wlll but I think we should change it later...
    internal class DefaultErrorInfoConverter : IExceptionToErrorInfoConverter
    {
        private readonly IADTOSharpWebCommonModuleConfiguration _configuration;
        private readonly ILocalizationManager _localizationManager;

        public IExceptionToErrorInfoConverter Next { set; private get; }

        private bool SendAllExceptionsToClients
        {
            get
            {
                return _configuration.SendAllExceptionsToClients;
            }
        }

        public DefaultErrorInfoConverter(
            IADTOSharpWebCommonModuleConfiguration configuration,
            ILocalizationManager localizationManager)
        {
            _configuration = configuration;
            _localizationManager = localizationManager;
        }

        public ErrorInfo Convert(Exception exception)
        {
            var errorInfo = CreateErrorInfoWithoutCode(exception);

            if (exception is IHasErrorCode)
            {
                errorInfo.Code = (exception as IHasErrorCode).Code;
            }

            return errorInfo;
        }

        private ErrorInfo CreateErrorInfoWithoutCode(Exception exception)
        {
            if (SendAllExceptionsToClients)
            {
                return CreateDetailedErrorInfoFromException(exception);
            }

            if (exception is AggregateException && exception.InnerException != null)
            {
                var aggException = exception as AggregateException;
                if (aggException.InnerException is UserFriendlyException ||
                    aggException.InnerException is ADTOSharpValidationException)
                {
                    exception = aggException.InnerException;
                }
            }

            if (exception is UserFriendlyException)
            {
                var userFriendlyException = exception as UserFriendlyException;
                return new ErrorInfo(userFriendlyException.Message, userFriendlyException.Details);
            }

            if (exception is ADTOSharpValidationException)
            {
                return new ErrorInfo(L("ValidationError"))
                {
                    ValidationErrors = GetValidationErrorInfos(exception as ADTOSharpValidationException),
                    Details = GetValidationErrorNarrative(exception as ADTOSharpValidationException)
                };
            }

            if (exception is EntityNotFoundException)
            {
                var entityNotFoundException = exception as EntityNotFoundException;

                if (entityNotFoundException.EntityType != null)
                {
                    return new ErrorInfo(
                        string.Format(
                            L("EntityNotFound"),
                            entityNotFoundException.EntityType.Name,
                            entityNotFoundException.Id
                        )
                    );
                }

                return new ErrorInfo(
                    entityNotFoundException.Message
                );
            }

            if (exception is ADTOSharp.Authorization.ADTOSharpAuthorizationException)
            {
                var authorizationException = exception as ADTOSharp.Authorization.ADTOSharpAuthorizationException;
                return new ErrorInfo(authorizationException.Message);
            }

            return new ErrorInfo(L("InternalServerError"));
        }

        private ErrorInfo CreateDetailedErrorInfoFromException(Exception exception)
        {
            var detailBuilder = new StringBuilder();

            AddExceptionToDetails(exception, detailBuilder);

            var errorInfo = new ErrorInfo(exception.Message, detailBuilder.ToString());

            if (exception is ADTOSharpValidationException)
            {
                errorInfo.ValidationErrors = GetValidationErrorInfos(exception as ADTOSharpValidationException);
            }

            return errorInfo;
        }

        private void AddExceptionToDetails(Exception exception, StringBuilder detailBuilder)
        {
            //Exception Message
            detailBuilder.AppendLine(exception.GetType().Name + ": " + exception.Message);

            //Additional info for UserFriendlyException
            if (exception is UserFriendlyException)
            {
                var userFriendlyException = exception as UserFriendlyException;
                if (!string.IsNullOrEmpty(userFriendlyException.Details))
                {
                    detailBuilder.AppendLine(userFriendlyException.Details);
                }
            }

            //Additional info for ADTOSharpValidationException
            if (exception is ADTOSharpValidationException)
            {
                var validationException = exception as ADTOSharpValidationException;
                if (validationException.ValidationErrors.Count > 0)
                {
                    detailBuilder.AppendLine(GetValidationErrorNarrative(validationException));
                }
            }

            //Exception StackTrace
            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                detailBuilder.AppendLine("STACK TRACE: " + exception.StackTrace);
            }

            //Inner exception
            if (exception.InnerException != null)
            {
                AddExceptionToDetails(exception.InnerException, detailBuilder);
            }

            //Inner exceptions for AggregateException
            if (exception is AggregateException)
            {
                var aggException = exception as AggregateException;
                if (aggException.InnerExceptions.IsNullOrEmpty())
                {
                    return;
                }

                foreach (var innerException in aggException.InnerExceptions)
                {
                    AddExceptionToDetails(innerException, detailBuilder);
                }
            }
        }

        private ValidationErrorInfo[] GetValidationErrorInfos(ADTOSharpValidationException validationException)
        {
            var validationErrorInfos = new List<ValidationErrorInfo>();

            foreach (var validationResult in validationException.ValidationErrors)
            {
                var validationError = new ValidationErrorInfo(validationResult.ErrorMessage);

                if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                {
                    validationError.Members = validationResult.MemberNames.Select(m => m.ToCamelCase()).ToArray();
                }

                validationErrorInfos.Add(validationError);
            }

            return validationErrorInfos.ToArray();
        }

        private string GetValidationErrorNarrative(ADTOSharpValidationException validationException)
        {
            var detailBuilder = new StringBuilder();
            detailBuilder.AppendLine(L("ValidationNarrativeTitle"));

            foreach (var validationResult in validationException.ValidationErrors)
            {
                detailBuilder.AppendFormat(" - {0}", validationResult.ErrorMessage);
                detailBuilder.AppendLine();
            }

            return detailBuilder.ToString();
        }

        private string L(string name)
        {
            try
            {
                return _localizationManager.GetString(ADTOSharpWebConsts.LocalizationSourceName, name);
            }
            catch (Exception)
            {
                return name;
            }
        }
    }
}