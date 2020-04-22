UPDATE [dbo].[protocollog] set logtype = 'PW' where id in (select pl.id from protocollog pl
inner join protocolrole pr on pr.year = pl.Year and pr.number = pl.number
inner join role r on r.idrole = pr.idrole
where pl.logtype='PO' and pl.logdescription like 'Spedito a ' + r.Name COLLATE SQL_Latin1_General_CP1_CI_AS + '%')


