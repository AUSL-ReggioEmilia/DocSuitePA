<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <head>
        <title>Rapporto lotti di versamento</title>
      </head>
      <body>
        <h2>            Rapporto lotti di versamento          </h2>
        <xsl:if test="count(/AwardBatchReport/Lotto) = 0">
          <p>Nessun lotto di versamento presente</p>
        </xsl:if>
        <xsl:for-each select="/AwardBatchReport/Lotto">
          <div style="margin-top:30px;">
            <h3>
              <xsl:value-of select="Descrizione" />
            </h3>
            <table style="width:100%;font-family:Verdana;font-size:small;" cellpadding="4">
              <tr style="font-weight:bold;background-color:#aaaaaa">
                <xsl:for-each select="File[1]">
                  <xsl:for-each select="Attributo">
                    <td>
                      <xsl:value-of select="Nome" />
                    </td>
                  </xsl:for-each>
                </xsl:for-each>
              </tr>
              <xsl:for-each select="File">
                <tr>
                  <xsl:if test="(position() mod 2 = 1)">
                    <xsl:attribute name="style">background-color:#eeeeee</xsl:attribute>
                  </xsl:if>
                  <xsl:for-each select="Attributo">
                    <td>
                      <xsl:choose>
                        <xsl:when test="Nome='NomeFileInArchivio'">
                          <a>
                            <xsl:attribute name="target">_blank</xsl:attribute>
                            <xsl:attribute name="href">
                              <xsl:value-of select="Valore" />
                            </xsl:attribute>
                            <xsl:value-of select="Valore" />
                          </a>
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="Valore" />
                        </xsl:otherwise>
                      </xsl:choose>
                    </td>
                  </xsl:for-each>
                </tr>
              </xsl:for-each>
            </table>
          </div>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>