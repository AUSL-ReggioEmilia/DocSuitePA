A new endpoint configuration is required in Web.config (BiblosDS.LegalExtension.AdminPortal),
in order to make the signing process working.
Please look into default_configuration folder

 <binding name="ArubaSignServicePortBinding" messageEncoding="Mtom" maxReceivedMessageSize="2147483647" maxBufferSize="2147483647" maxBufferPoolSize="2147483647">
          <readerQuotas maxDepth="32" maxArrayLength="2147483647" maxStringContentLength="2147483647"/>
          <security mode="Transport" />
 </binding>

<endpoint address="https://arss-land.actalis.it:443/ArubaSignService/ArubaSignService"
binding="basicHttpBinding" bindingConfiguration="ArubaSignServicePortBinding"
contract="Aruba.ArubaSignService" name="ArubaSignServicePort" />  

To add a new signing profile for a Company you need to access the dbo.Company table in the database.
To add a new signing profile for a Customer you need to access the ext.Customer table in the database.

In the SignInfo column, you'll have to add the following json structure, with your specific credentials.

{
  "DelegatedDomain": "<your_delegate_domain>",
  "DelegatedPassword": "<your_delegated_password>",
  "DelegatedUser": "<your_delegated_user>",
  "OTPPassword": "<your_OTP_password>",
  "OTPAuthType": "<your_OTP_Authentication_Type>",
  "User": "<your_user>",
  "CertificateId": "your_certificate_id",
  "TSAUser": "tsa_user marca temporale",
  "TSAPassword": "tra_password marca temporale"
}

