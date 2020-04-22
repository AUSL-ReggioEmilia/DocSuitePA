<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/VerifyReport">
    <html>
      <body style="font-size:8px;">

        <h3>
          Rapporto verifica Conservazioni
        </h3>

        <div>
          Verifica eseguita su conservazioni chiuse dalla data
          <b>
            <xsl:value-of select="fromDate" />
          </b>
          alla data
          <b>
            <xsl:value-of select="toDate" />
          </b>
        </div>

        <xsl:if test="count(/VerifyReport/Archive) = 0">
          <p>Nessun archivio selezionato</p>
        </xsl:if>

        <xsl:for-each select="/VerifyReport/Archive">
          <br></br>
          <b>
            Archivio  <xsl:value-of select="ArchiveName" />
          </b>
          <xsl:if test="VerifyReportPreservation/IdPreservation != '00000000-0000-0000-0000-000000000000'">
            - (<xsl:value-of select="count(VerifyReportPreservation)"  /> conservazioni)
          </xsl:if>
          <table>
            <xsl:for-each select="VerifyReportPreservation">
              <tr>
                <td>
                  <xsl:value-of select="Name" />
                </td>
                <td>
                  &#160;
                  <xsl:if test="IdPreservation != '00000000-0000-0000-0000-000000000000'">
                    <xsl:value-of select="Result" />
                  </xsl:if>
                </td>
              </tr>
            </xsl:for-each>
          </table>
        </xsl:for-each>

        <br></br>
        <div style="text-align:center;">
          Data esecuzione: <xsl:value-of select="verifyDate" /> - Biblos preservation portal - Dgroove Srl
        </div>

      </body>

    </html>
  </xsl:template>
</xsl:stylesheet>