import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ProtocolModel } from '../../models/protocols/protocol.model';
import { ProtocolStatusType } from '../../models/protocols/protocol-status.type';
import { ComunicationType } from '../../models/protocols/comunication.type';
import { ProtocolContactModel } from '../../models/protocols/protocol-contact.model';
import { ResponseModel } from '../../models/commons/response.model';
import { TreeListModel } from '../../models/commons/tree-list.model';
import { SectorTreeListMapper } from '../../mappers/commons/sector-tree-list.mapper';
import { UserTreeListMapper } from '../../mappers/commons/user-tree-list.mapper';
import { ContactTreeListMapper } from '../../mappers/commons/contact-tree-list.mapper';
import { BaseHelper } from '../../helpers/base.helper';
import { ErrorLogModel } from '../../models/commons/error-log.model';
import { AppConfigService } from '../../services/commons/app-config.service';
import { LoadingSpinnerService } from '../../services/commons/loading-spinner.service';
import { ISubscription } from 'rxjs/Subscription';
import { ProtocolService } from '../../services/protocols/protocol.service';

@Component({
    moduleId: module.id,
    templateUrl: 'protocol-summary.component.html',
    styleUrls: ['protocol-summary.component.css']
})

export class ProtocolSummaryComponent implements OnInit, OnDestroy {

    response: ResponseModel<ProtocolModel>;
    pecOutgoingCount: number = 0;
    pecIngoingCount: number = 0;
    protocol: ProtocolModel;
    sectors: TreeListModel[];
    senders: TreeListModel[];
    recipients: TreeListModel[];
    sendersTitle: string;
    recipientsTitle: string;

    annulmentImage: string = 'app/images/annulment.png';
    rejectImage: string = 'app/images/reject.png';
    rejectReason: string;

    userIcon: string = 'app/images/sectors/user.png';
    resolverSubscription: ISubscription;
    resolverRouterSubscription: ISubscription;

    constructor(private protocolService: ProtocolService, private route: ActivatedRoute, private router: Router,
        private baseHelper: BaseHelper, private toastr: ToastrService, private spinnerService: LoadingSpinnerService) {
        this.resolverRouterSubscription = this.router.events.subscribe((e: any) => {
            // If it is a NavigationEnd event re-initalise the component
            if (e instanceof NavigationEnd) {
                this.ngOnInit();
            }
        });
    }

    ngOnInit(): void {
        let year: number = +this.route.params['value']['year'];
        let number: number = +this.route.params['value']['number'];
        this.resolverSubscription = this.protocolService.getProtocolSummary(year, number).subscribe(response => this.successCallback(response), this.errorCallback);
    }

    ngOnDestroy(): void {
        this.resolverSubscription.unsubscribe();
        this.resolverRouterSubscription.unsubscribe();
    }


    successCallback(response: ResponseModel<ProtocolModel>) {
        //TODO: binding dei risultati e verifica degli eventuali errori gestiti
        this.response = response;

        if (response && response.results[0] && response.results.length == 1) {
            this.protocol = response.results[0];
            let sectorMapper: SectorTreeListMapper = new SectorTreeListMapper();
            let contactMapper: ContactTreeListMapper<ProtocolContactModel> = new ContactTreeListMapper(this.baseHelper);
            this.sectors = !!this.protocol.sectors ? this.protocol.sectors.map(sector => sectorMapper.mapToTreeList(sector)) : null;
            if (this.protocol.users && this.protocol.users.length > 0) {
                let userMapper: UserTreeListMapper = new UserTreeListMapper();
                let users: TreeListModel[] = this.protocol.users.map(user => userMapper.mapToTreeList(user));
                for (let item of users) {
                    this.sectors.push(item);
                }
            }

            this.senders = !!this.protocol.contacts ? this.protocol.contacts.filter((value) => {
                return value.comunicationType == ComunicationType.Sender;
            }).map(contact => contactMapper.mapToTreeList(contact)) : null;

            this.recipients = !!this.protocol.contacts ? this.protocol.contacts.filter((value) => {
                return value.comunicationType == ComunicationType.Recipient;
            }).map(contact => contactMapper.mapToTreeList(contact)) : null;

            this.protocol.isRejected = this.protocol.status == ProtocolStatusType.Rejected;
            this.rejectReason = !this.protocol.annulmentReason ? 'Nessun motivo esplicito.' : this.protocol.annulmentReason;

            this.protocolService.getProtocolOutgoingPECCount(this.protocol.year, this.protocol.number).subscribe(response => {
                if (response != null) {
                    this.pecOutgoingCount = response;
                }
            }, this.errorCallback)

            this.protocolService.getProtocolIngoingPECCount(this.protocol.year, this.protocol.number).subscribe(response => {
                if (response != null) {
                    this.pecIngoingCount = response;
                }
            }, this.errorCallback)

        }

        this.spinnerService.stop();
    }

    errorCallback = (error): void => {
        //TODO: toast notification per eccezione critica
        this.toastr.clear();
        this.spinnerService.stop();
        console.log(error);
        let errorMessage: any;
        if (error.constructor.name == 'ErrorLogModel') {
            let errorLog: ErrorLogModel = error as ErrorLogModel;
            errorMessage = errorLog.status.toString().concat(' - ', errorLog.errorMessages[0]);
        }
        else {
            errorMessage = error;
        }
        this.toastr.error(errorMessage, 'Errore!', { timeOut: AppConfigService.settings.toastLife });
    }

}