<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Cargar-Calificaciones.aspx.cs" Inherits="Cargar_Calificaciones" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form runat="server">
        <div class="col-xs-12">
            <div class="card">
                <div class="card-body">
                    <asp:FileUpload ID="fuArchivo" runat="server" />
                    <br />
                    <br />
                    <asp:Button runat="server" class="btn btn-success" ID="btnSubir" Text="Subir" OnClick="btnSubir_Click" />
                    <br />
                    <br />
                    <input type="button" class="btn btn-success" visible="false" id="btnGenerarInformacion" runat="server" value="Generar Informaciòn" />
                    <asp:Label runat="server" ID="lblStatus" /><br />
                    <br />
                </div>
            </div>
        </div>
        <div id="divGraficas" class="col-lg-12">
            <div>
                    <label id="lblGraficaInformacion" />
            </div>
            <div style="height: 450px; width: 400px">
                <canvas id="cnvCalificaciones" width="400" height="400"></canvas>
            </div>
             <div>
                    <label id="lblGraficaPromedio" />
            </div>
            <div style="height: 450px; width: 400px">
                <canvas id="cnvPromedio" width="400" height="400"></canvas>
            </div>
        </div>

        <div class="col-xs-12" runat="server" id="divMenciones">
            <div class="card">
                <div class="card-body">

                    <div>
                        <b><label id="lblMejorCalificacion" /></b><br />
                    </div>
                    <div>
                        <b><label id="lblMenorCalificacion" /> </b><br />
                    </div>
                    <div>
                        <b><label id="lblPromedioCalificacion" /></b><br />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
<script src="js/jquery-1.10.2.js"></script>
<script src="js/bootstrap.js"></script>
<script src="js/Chart.js"></script>
<script src="js/Datos.js"></script>
</html>
