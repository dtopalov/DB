<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <style>
      	div{
      		padding: 5px;
      	}
        #table {
	        font-family: Arial;
	        display: table;
	        border-collapse: collapse;
        }

        #th-group{
	        display: table-header-group;
	        padding: 5px;
	        text-align: center;
	        font-weight: bold;
        }

        #tbody{
        	display: table-row-group
        }

        .cell{
	        display: table-cell;
	        border: 1px solid black;
        }

        .inner-cell{
			border: none;
        }

        .row{
        	display: table-row;
        }

      </style>

      <body>
        <div id="table">
          <div id="th-group">
            <div class="cell">Name</div>
            <div class="cell">Gender</div>
            <div class="cell">Date of Birth</div>
            <div class="cell">Phone</div>
            <div class="cell">E-mail</div>
            <div class="cell">Course</div>
            <div class="cell">Specialty</div>
            <div class="cell">Faculty#</div>
            <div class="cell">Exams</div>
            <div class="cell">Enrollment</div>
            <div class="cell">Endorsements</div>
          </div>
          <div id="tbody">
            <xsl:for-each select="students/student">
              <div class="row">
                <div class="cell">
                  <xsl:value-of select="name" />
                </div>
                <div class="cell">
                  <xsl:value-of select="sex" />
                </div>
                <div class="cell">
                  <xsl:value-of select="birthdate" />
                </div>
                <div class="cell">
                  <xsl:value-of select="phone" />
                </div>
                <div class="cell">
                  <xsl:value-of select="email" />
                </div>
                <div class="cell">
                  <xsl:value-of select="course" />
                </div>
                <div class="cell">
                  <xsl:value-of select="specialty" />
                </div>
                <div class="cell">
                  <xsl:value-of select="faculty-number" />
                </div>
                <div class="cell">
                  <xsl:for-each select="exams/exam">
                    <div class="inner-cell">
                      <strong>
                        <xsl:value-of select="exam-name" /><br />
                      </strong>
                      tutor: <xsl:value-of select="tutor"/><br /> 
                      score: <xsl:value-of select="score" />
                    </div>
                  </xsl:for-each>
                </div>
                <div class="cell">
                  Date: <xsl:value-of select="enrollment-info/enrollment-date" /><br />
                  Score: <xsl:value-of select="enrollment-info/exam-score" />
                </div>
                <div class="cell">
                  <xsl:for-each select="teacher-endorsements/teacher-endorsement">
                    <div>
                      <strong>
                        <xsl:value-of select="teacher-name" />: 
                      </strong>
                      <xsl:value-of select="endorsement" />
                    </div>
                  </xsl:for-each>
                </div>
              </div>
            </xsl:for-each>
          </div>  
        </div>
      </body>
    </html>

  </xsl:template>
</xsl:stylesheet>
