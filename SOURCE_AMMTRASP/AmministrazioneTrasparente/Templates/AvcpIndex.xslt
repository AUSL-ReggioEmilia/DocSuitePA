<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    exclude-result-prefixes="msxsl"
    xmlns:ms="urn:schemas-microsoft-com:xslt"
>
  <xsl:output encoding="utf-8" indent="yes" method="xml" omit-xml-declaration="yes"/>
  <xsl:param name="html-content-type"/>

  <xsl:template match="/">
    <xsl:param name="siteUrl" />
    <html lang="en">
      <head>
        <title>Amministrazione Trasparente - Indice Pubblicazione</title>
        <link rel="stylesheet" href="css/bootstrap.css" />
        <meta charset="utf-8" />
        <meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <style>
          body{
            padding-top: 50px;
          }
        </style>
      </head>
      <body>        
        <nav class="navbar navbar-default navbar-fixed-top">
          <div class="container">
            <div class="navbar-header">
              <a class="navbar-brand" href="{$siteUrl}">Amministrazione Trasparente</a>
            </div>
          </div>
        </nav>
        <div class="container">
          <xsl:apply-templates select="indici"/>
        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="/Indici">
    <Indici>
      <xsl:apply-templates select="metadata"/>
      <xsl:apply-templates select="indice"/>
    </Indici>
  </xsl:template>

  <xsl:template match="metadata">
    <div class="page-header">
      <h1>
        <xsl:value-of select="titolo"/>
      </h1>
      <dl>
        <dt>Data Pubblicazione Indice</dt>
        <dd>
          <xsl:value-of select="ms:format-date(dataPubblicazioneIndice, 'dd/MM/yyyy')"/>
        </dd>
        <dt>Ente Pubblicatore</dt>
        <dd>
          <xsl:value-of select="entePubblicatore"/>
        </dd>
        <dt>Data Ultimo Aggiornamento Indice</dt>
        <dd>
          <xsl:value-of select="ms:format-date(dataUltimoAggiornamentoIndice, 'dd/MM/yyyy')"/>
        </dd>
        <dt>Anno Riferimento</dt>
        <dd>
          <xsl:value-of select="annoRiferimento"/>
        </dd>
      </dl>
    </div>
  </xsl:template>

  <xsl:template match="indice">
    <table class="table table-striped table-hover">
      <thead>
        <tr>
          <th>Link</th>
          <th>Data Ultimo Aggiornamento</th>
        </tr>
      </thead>
      <xsl:for-each select="dataset">
        <xsl:sort select="dataUltimoAggiornamento" order="descending" />
        <tr>
          <td>
            <a>
              <xsl:variable name="linkDataset" select="linkDataset"/>
              <a href="{$linkDataset}" target='_blank'>
                <xsl:value-of select="linkDataset" />
              </a>
            </a>
          </td>
          <td>
            <xsl:value-of select="ms:format-date(dataUltimoAggiornamento, 'dd/MM/yyyy')"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>
</xsl:stylesheet>
