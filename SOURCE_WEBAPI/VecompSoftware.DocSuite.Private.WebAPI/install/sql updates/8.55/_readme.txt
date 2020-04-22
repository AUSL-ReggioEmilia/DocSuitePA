Assicurarsi che l'utente con cui le Web API si collegano a SQL, abbia diritti di EXECUTE per le Stored Procedure.
rif. https://msdn.microsoft.com/it-it/library/ms188371.aspx

N.B. Se l'utente di connessione a SQL è DB owner non è necessario verificare i diritti di EXECUTE.

#AC
#########################################
Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - EnterpriseLibrary.Validation.config
 - WebApi.appSettings.config
 - WebApi.connectionStrings.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
###############################################################

Il file EnterpriseLibrary.Validation.config è stato modificato introducendo delle nuove logiche di validazione per il
massimario di scarto.
E' quindi possibile procedere alla modifica del file in 2 modi 

(NOTA MESSA PER INTERESSE, MA SI PUO' PRELEVARE QUELLO DI DEFAULT GIA' COMPLETO. ATTENZIONE IN AUSL-PC ALLA VALIDAZIONE SPECIFICA DEI FASCICOLI, VEDI README 8.52)

1) Enterprise Library Tool: 
	- Aggiungere un nuovo validator "MassimarioScartoValidator" (icona + -> Add type to validate).
	- Associare 2 nuovi ruleset "MassimarioScartoInsert" e "MassimarioScartoUpdate".
	
	MassimarioScartoInsert
		- Per il ruleset "MassimarioScartoInsert" aggiungere la proprietà "StartDate" da validare.
		- Per la proprietà StartDate aggiungere un "Or Composite Validator" a cui andranno aggiunti i validators:			
		- Per il ruleset "MassimarioScartoInsert" aggiungere la proprietà "EndDate" da validare.
			- Property Comparison Validator {Comparison Operator = "GreatherThan", Message Template = "La data di disattivazione deve essere maggiore della data attivazione", 
												Negated = False, Property To Compare = "StartDate" }.
			- Not Null Validator {Negated = True}
		- Per il ruleset "MassimarioScartoInsert" aggiungere la proprietà "Name" da validare.
			- Not Null Validator {Negated = False, Message Template = "La proprietà Name non può essere vuota."}.
		- Per il ruleset "MassimarioScartoInsert" aggiungere la proprietà "Code" da validare.
			- Not Null Validator {Negated = False, Message Template = "La proprietà Code non può essere vuota."}.
		- Per il ruleset "MassimarioScartoInsert" aggiungere la proprietà "Self" da validare.
			- Custom Validator "HasValidStartDate" {Message Template = "La data di attivazione non può essere antecedente alla data di attivazione del nodo padre."}.

	MassimarioScartoUpdate
		- Per il ruleset "MassimarioScartoUpdate" aggiungere la proprietà "StartDate" da validare.
		- Per la proprietà StartDate aggiungere un "Or Composite Validator" a cui andranno aggiunti i validators:			
		- Per il ruleset "MassimarioScartoUpdate" aggiungere la proprietà "EndDate" da validare.
			- Property Comparison Validator {Comparison Operator = "GreatherThan", Message Template = "La data di disattivazione deve essere maggiore della data attivazione", 
												Negated = False, Property To Compare = "StartDate" }.
			- Not Null Validator {Negated = True}
		- Per il ruleset "MassimarioScartoUpdate" aggiungere la proprietà "Name" da validare.
			- Not Null Validator {Negated = False, Message Template = "La proprietà Name non può essere vuota."}.
		- Per il ruleset "MassimarioScartoUpdate" aggiungere la proprietà "Code" da validare.
			- Not Null Validator {Negated = False, Message Template = "La proprietà Code non può essere vuota."}.
		- Per il ruleset "MassimarioScartoUpdate" aggiungere la proprietà "Self" da validare.
			- Custom Validator "HasValidStartDate" {Message Template = "La data di attivazione non può essere antecedente alla data di attivazione del nodo padre.".

	Fare attenzione perchè il tool al salvataggio inserirà tutti i valori nel web.config. Provvedere successivamente a spostarli nel file EnterpriseLibrary.Validation.config.

2) Modifica diretta del file EnterpriseLibrari.Validation.config
	Aggiungere la seguente sezione al file:
		<type name="VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.MassimariScarto.MassimarioScartoValidator"
    defaultRuleset="MassimarioScartoInsert" assemblyName="VecompSoftware.DocSuiteWeb.Validation, Version=8.55.0.0, Culture=neutral, PublicKeyToken=null">
    <ruleset name="MassimarioScartoInsert">
      <properties>
        <property name="StartDate" />
        <property name="EndDate">
          <validator type="Microsoft.Practices.EnterpriseLibrary.Validation.Validators.OrCompositeValidator, Microsoft.Practices.EnterpriseLibrary.Validation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            name="Or Composite Validator">
            <validator type="Microsoft.Practices.EnterpriseLibrary.Validation.Validators.PropertyComparisonValidator, Microsoft.Practices.EnterpriseLibrary.Validation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
              operator="GreaterThan" propertyToCompare="StartDate" messageTemplate="La data di disattivazione deve essere maggiore della data attivazione"
              name="Property Comparison Validator" />
            <validator type="Microsoft.Practices.EnterpriseLibrary.Validation.Validators.NotNullValidator, Microsoft.Practices.EnterpriseLibrary.Validation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
              negated="true" name="Not Null Validator" />
          </validator>
        </property>
        <property name="Name">
          <validator type="Microsoft.Practices.EnterpriseLibrary.Validation.Validators.NotNullValidator, Microsoft.Practices.EnterpriseLibrary.Validation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            messageTemplate="La proprietà Name non può essere vuota." name="Not Null Validator" />
        </property>
        <property name="Code">
          <validator type="Microsoft.Practices.EnterpriseLibrary.Validation.Validators.NotNullValidator, Microsoft.Practices.EnterpriseLibrary.Validation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            messageTemplate="La proprietà Code non può essere vuota." name="Not Null Validator" />
        </property>
        <property name="Self">
          <validator type="VecompSoftware.DocSuiteWeb.CustomValidation.Entities.MassimariScarto.HasValidStartDate, VecompSoftware.DocSuiteWeb.CustomValidation, Version=8.55.0.0, Culture=neutral, PublicKeyToken=null"
            messageTemplate="La data di attivazione non può essere antecedente alla data di attivazione del nodo padre."
            name="HasValidStartDate" />
        </property>
      </properties>
    </ruleset>
    <ruleset name="MassimarioScartoUpdate">
      <properties>
        <property name="StartDate" />
        <property name="EndDate">
          <validator type="Microsoft.Practices.EnterpriseLibrary.Validation.Validators.OrCompositeValidator, Microsoft.Practices.EnterpriseLibrary.Validation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            name="Or Composite Validator">
            <validator type="Microsoft.Practices.EnterpriseLibrary.Validation.Validators.PropertyComparisonValidator, Microsoft.Practices.EnterpriseLibrary.Validation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
              operator="GreaterThan" propertyToCompare="StartDate" messageTemplate="La data di disattivazione deve essere maggiore della data attivazione"
              name="Property Comparison Validator" />
            <validator type="Microsoft.Practices.EnterpriseLibrary.Validation.Validators.NotNullValidator, Microsoft.Practices.EnterpriseLibrary.Validation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
              negated="true" name="Not Null Validator" />
          </validator>
        </property>
        <property name="Name">
          <validator type="Microsoft.Practices.EnterpriseLibrary.Validation.Validators.NotNullValidator, Microsoft.Practices.EnterpriseLibrary.Validation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            messageTemplate="La proprietà Name non può essere vuota." name="Not Null Validator" />
        </property>
        <property name="Code">
          <validator type="Microsoft.Practices.EnterpriseLibrary.Validation.Validators.NotNullValidator, Microsoft.Practices.EnterpriseLibrary.Validation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            messageTemplate="La proprietà Code non può essere vuota." name="Not Null Validator" />
        </property>
        <property name="Self">
          <validator type="VecompSoftware.DocSuiteWeb.CustomValidation.Entities.MassimariScarto.HasValidStartDate, VecompSoftware.DocSuiteWeb.CustomValidation, Version=8.55.0.0, Culture=neutral, PublicKeyToken=null"
            messageTemplate="La data di attivazione non può essere antecedente alla data di attivazione del nodo padre."
            name="HasValidStartDate" />
        </property>
      </properties>
    </ruleset>
  </type>

#AC
#########################################