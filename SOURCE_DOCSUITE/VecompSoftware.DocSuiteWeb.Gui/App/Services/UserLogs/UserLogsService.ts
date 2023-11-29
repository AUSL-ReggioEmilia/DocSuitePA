import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UserLogsMapper = require('../../Mappers/UserLogs/UserLogsMapper');
import UserLogsModel = require('../../Models/UserLogs/UserLogsModel');

class UserLogsService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    //The method will return an encrypted UserProfile
    getUserDetailBySystemUser(name: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = `${this._configuration.ODATAUrl}?$filter=SystemUser eq '${name}'`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    let modelMapper = new UserLogsMapper();
                    let userLogs: UserLogsModel[] = [];
                    $.each(response.value, function (i, value) {
                        userLogs.push(modelMapper.Map(value));
                    });

                    callback(userLogs);
                }
            }, error);
    }

    //The method will return the current UserLog with decrypted UserProfile but with the PIN, Password and OTP information blanked out
    getCurrentUserDetailSecure(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = `${this._configuration.ODATAUrl}/UserLogService.GetCurrentUserLogSecure()`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    let modelMapper = new UserLogsMapper();
                    let userLog = new UserLogsModel();
                    userLog = modelMapper.Map(response, true);
                    callback(userLog);
                }
            }, error);
    }
}

export = UserLogsService;