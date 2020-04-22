<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>Lista</title>
			</head>
			<body>
				<table style="width:100%;font-family:Verdana;font-size:small;" cellpadding="4">
					<tr style="font-weight:bold;background-color:#aaaaaa">
						<td>Num.</td>
						<td>DataFattura</td>
						<td>NumeroFattura</td>
						<td>File</td>
						<td>Denominazione</td>
						<td>PartitaIVA</td>
						<td>Nome</td>
						<td>Cognome</td>
						<td>CodiceFiscale</td>
						<td>Impronta SHA1 file</td>
					</tr>
					<xsl:for-each select="Indice/File">
						<tr>
							<xsl:if test="(position() mod 2 = 1)">
								<xsl:attribute name="style">background-color:#eeeeee</xsl:attribute>
							</xsl:if>
							<td>
								<xsl:value-of select="@Progressivo" />
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="count(Attributo[@Nome='DataFattura']/child::node()) = 1">
										<xsl:value-of select="Attributo[@Nome='DataFattura']" />
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="count(Attributo[@Nome='NumeroFattura']/child::node()) = 1">
										<xsl:value-of select="Attributo[@Nome='NumeroFattura']" />
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<a>
									<xsl:attribute name="target">_blank</xsl:attribute>
									<xsl:attribute name="href">
										<xsl:value-of select="Attributo[@Nome='NomeFileInArchivio']" />
									</xsl:attribute>
									<xsl:value-of select="Attributo[@Nome='NomeFileInArchivio']" />
								</a>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="count(Attributo[@Nome='Denominazione']/child::node()) = 1">
										<xsl:value-of select="Attributo[@Nome='Denominazione']" />
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="count(Attributo[@Nome='PartitaIVA']/child::node()) = 1">
										<xsl:value-of select="Attributo[@Nome='PartitaIVA']" />
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="count(Attributo[@Nome='Nome']/child::node()) = 1">
										<xsl:value-of select="Attributo[@Nome='Nome']" />
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="count(Attributo[@Nome='Cognome']/child::node()) = 1">
										<xsl:value-of select="Attributo[@Nome='Cognome']" />
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="count(Attributo[@Nome='CodiceFiscale']/child::node()) = 1">
										<xsl:value-of select="Attributo[@Nome='CodiceFiscale']" />
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="count(Attributo[@Nome='ImprontaFileSHA1']/child::node()) = 1">
										<xsl:value-of select="Attributo[@Nome='ImprontaFileSHA1']" />
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
