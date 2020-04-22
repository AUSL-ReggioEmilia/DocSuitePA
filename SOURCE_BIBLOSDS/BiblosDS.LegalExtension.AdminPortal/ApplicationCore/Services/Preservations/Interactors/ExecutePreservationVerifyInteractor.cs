using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Preservations;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.Preservations.Interactors
{
    public class ExecutePreservationVerifyInteractor : IInteractor<ExecutePreservationVerifyRequestModel, ExecutePreservationVerifyResponseModel>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly PreservationService _preservationService;
        #endregion

        #region [ Constructor ]
        public ExecutePreservationVerifyInteractor(ILogger logger)
        {
            _logger = logger;
            _preservationService = new PreservationService();
        }
        #endregion

        #region [ Methods ]
        public ExecutePreservationVerifyResponseModel Process(ExecutePreservationVerifyRequestModel request)
        {            
            try
            {
                _logger.Info($"Process -> execute verify for preservation {request.IdPreservation}");
                ExecutePreservationVerifyResponseModel responseModel = new ExecutePreservationVerifyResponseModel(request.IdPreservation);
                if (request.IdPreservation == Guid.Empty)
                {
                    return responseModel;
                }

                Preservation preservation = _preservationService.GetPreservation(request.IdPreservation, false);
                if (preservation == null)
                {
                    _logger.Warn($"Process -> preservation with id {request.IdPreservation} not found");
                    responseModel.Status = PreservationVerifyStatus.Error;
                    responseModel.Errors.Add($"Nessuna conservazione trovata con ID {request.IdPreservation}");
                    return responseModel;
                }

                _preservationService.ArchiveConfigFile = ConfigurationHelper.GetArchiveConfigurationFilePath(preservation.Archive.Name);
                bool isVerified = _preservationService.VerifyExistingPreservation(request.IdPreservation);

                string[] tokens = Path.GetFileName(_preservationService.VerifyFile).Split(' ');
                string title = Path.GetFileNameWithoutExtension(String.Join(" ", tokens, 1, tokens.Length - 1));

                responseModel.Status = isVerified ? PreservationVerifyStatus.Ok : PreservationVerifyStatus.Error;
                responseModel.VerifyTitle = title;
                responseModel.Errors = _preservationService.ErrorMessages;
                _logger.Info($"Process -> preservation is verified with status {responseModel.Status.ToString()}");
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error on verify preservation {request.IdPreservation}", ex);
                return new ExecutePreservationVerifyResponseModel(request.IdPreservation)
                {
                    Status = PreservationVerifyStatus.Error,
                    Errors = new List<string>()
                    {
                        ex.Message
                    }
                };
            }            
        }
        #endregion        
    }
}