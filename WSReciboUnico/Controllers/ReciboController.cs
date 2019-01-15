using Datos;
using Entidades;
using Rotativa.MVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using ReciboUnico;
using Newtonsoft.Json;
using System.Web;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using System.Security.Policy;
using System.Security.Cryptography;
using SSO_Login;
using WSReciboUnico.Models;

namespace WSReciboUnico.Controllers
{

    public class ReciboController : ApiController
    {


        public string GetRecibo()
        {
            string respuesta = "";
            try
            {
                var Url = HttpContext.Current.Request.RawUrl;
                var urldecode = Url.Substring(Url.LastIndexOf("?") + 1);
                urldecode = Encryption.RSADecrypt(urldecode);

                var _parameters = JsonConvert.DeserializeObject<WARecibo>(urldecode);

                //var _parameters = new WARecibo { iacx = _iacx, referenciax = _referenciax, valorx = _valorx, fechax = _fechax, idpersona = _idpersona, idproducto = _idproducto, usuario_wa = _usuario_wa, password_wa = _password_wa };
                //var _params = JsonConvert.SerializeObject(_parameters);
                //var hghgh = Encryption.RSAEncrypt(_params);
                //var _bytes = Encoding.UTF8.GetByteCount(_params);
                //var ser = Encryption.RSAEncrypt(_params);

                //WARecibo Parameters = (WARecibo)JsonConvert.DeserializeObject(_parameters);
                //HttpContext.Current.Request. = Url + urlEncrypt;
                Logica.ReciboLN reciboLN = new Logica.ReciboLN();
                UsuarioWA usr = new UsuarioWA();
                usr.IDPRODUCTO = _parameters.idproducto;
                usr.USUARIO_WA = _parameters.usuario_wa;
                usr.PASSWORD_WA = _parameters.password_wa;
                
                usr = reciboLN.Validar(usr);
                
                if (usr.VALIDO)
                {
                    var tipopersona = _parameters.tipopersona;


                    respuesta = "Generado";
                    ReciboUnico.Controllers.ReciboController Rcontrol = new ReciboUnico.Controllers.ReciboController();
                    ViewResult Resultado = (ViewResult)Rcontrol.ReciboUnico(_parameters.valorx, _parameters.fechax, _parameters.idpersona, tipopersona,_parameters.IdProgramaBeca);
                    Entidades.Recibo modelo = (Entidades.Recibo)Resultado.ViewData.Model;
                    string path = HttpContext.Current.Server.MapPath(@"~/Views/Recibo/");
                    System.Net.WebClient wc = new System.Net.WebClient();
                    string Urlcombi = "http://10.1.4.241/librerias/GenerarCodigoBarras/Default.aspx?IAC=" + _parameters.iacx + "&REFERENCIA=" + _parameters.referenciax + "&VALOR=" + _parameters.valorx + "&FECHA=" + _parameters.fechax;
                    WebRequest request = WebRequest.Create(Urlcombi);
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string str = reader.ReadLine();
                    string htmlpdf = string.Format(@"<div><div align='center'>
        <table width='100%' border='0' cellspacing='0'>
            <tr>
                <td>
                    <p align='center'>
                        <font size='2' face='Verdana, Arial, Helvetica, sans-serif'>
                            <strong>FONDO ESPECIAL BECAS SER </strong><br>
                            Artículo 104 Ley 1819 de 2016 <br> Decreto XXXX de 2018 / Resolución XXX de 2018<br>
                        </font>
                </td>
            </tr>
        </table>
        <br>

        <table width='649' border='1' cellpadding='0' cellspacing='0' bordercolor='#000000'>
            <tr>
                <td colspan='2' rowspan='4'>
                    <div align='center'>
                        <img src='http://10.1.18.120/ApiRecibo/Content/LOGO.JPG' width='155' height='38'><br>
                        <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            NIT 899.999.035-7<br>
                            Recibo de Pago No. {10}

                        </font>
                    </div>
                </td>
                <td width='98' class='LETRAPEQUE&Ntilde;A'>Razon Social</td>
                <td width='170' class='LETRAPEQUE&Ntilde;A'><font size='2' face='Verdana, Arial, Helvetica, sans-serif'>{0}</font></td>
                <td width='201' rowspan='5' valign='top'>
                    <div align='center'>
                        <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            TIMBRE CAJERO<br>(Copia Depositante)<br>
                        </font>
                    </div>
                </td>
            </tr>
            <tr>
                <td class='LETRAPEQUE&Ntilde;A'>Direcci&oacute;n</td>
                <td class='LETRAPEQUE&Ntilde;A'>{1}</td>
            </tr>
            <tr>
                <td class='LETRAPEQUE&Ntilde;A'>Tel&eacute;fonos</td>
                <td class='LETRAPEQUE&Ntilde;A'>{2}</td>
            </tr>
            <tr>
                <td class='LETRAPEQUE&Ntilde;A'>Ciudad</td>
                <td class='LETRAPEQUE&Ntilde;A'>{3}</td>
            </tr>
            <tr>
                <td height='53' colspan='4'>
                    <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                        Concepto de Este Pago
                    </font>:<span class=''>
                        <font size='2' face='Verdana, Arial, Helvetica, sans-serif'> Donacion - {0} - Programa: {4}, Valor: <strong>{7}</strong></font>
                    </span>
                </td>
            </tr>
            <tr>
                <td colspan='5' bgcolor='#999999'>
                    <div align='center'>
                        <font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            <strong>
                                ENTIDADES
                                BANCARIAS AUTORIZADAS
                            </strong>
                        </font>
                    </div>
                </td>
            </tr>
            <tr class=''>

                <td colspan='2'> <div align='center'><font size='2' face='Verdana, Arial, Helvetica, sans-serif'>BANCOLOMBIA - BANCO AGRARIO </font></div></td>
                <td colspan='2'> <div align='center'><font size='2' face='Verdana, Arial, Helvetica, sans-serif'>BBVA - BCSC CAJA SOCIAL </font></div></td>
                <td> <div align='center'><font size='2' face='Verdana, Arial, Helvetica, sans-serif'>HELM BANK - POPULAR - OCCIDENTE</font></div></td>
            </tr>
            <tr>
                <td colspan='5' bgcolor='#999999'>
                    <div align='center'><font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'></font></div>
                    <div align='center'>
                        <font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            <strong>
                                DETALLE
                                DEL PAGO
                            </strong>
                        </font>
                    </div>
                </td>
            </tr>
            <tr>
                <td width='154' height='40' valign='top'>
                    <div align='center'>
                        <strong>
                            <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                                PAGO
                                OPORTUNO HASTA<br>
                                {5}
                            </font>
                        </strong>
                    </div>
                </td>
                <td height='40' colspan='2' valign='top'>
                    <div align='center'>
                        <strong>
                            <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                                FECHA
                                DE PAGO<br>
                                aaaa/mm/dd
                            </font>
                        </strong>
                    </div>
                </td>
                <td colspan='2' valign='middle'>
                    <div align='center'>
                        <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            <strong>
                                C&oacute;digo
                                Referencia<br>
                                <font size='2'> </font>
                            </strong>
                        </font><font size='2' face='Verdana, Arial, Helvetica, sans-serif'>
                            Fondo Becaser {6}
                        </font>
                    </div>
                </td>
            </tr>

            <tr>
                <td bgcolor='#999999'>
                    <div align='center'>
                        <strong>
                            <font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                                C&oacute;digo
                                Banco
                            </font>
                        </strong>
                    </div>
                </td>
                <td colspan='3' bgcolor='#999999'>
                    <div align='center'>
                        <strong>
                            <font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                                Cheque
                                de Gerencia No.
                            </font>
                        </strong>
                    </div>
                    <div align='center'><strong></strong></div>
                </td>
                <td bgcolor='#999999'> <div align='center'><strong><font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>Valor</font></strong></div></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan='3'>&nbsp;</td>
                <td class='LETRAPEQUE&Ntilde;A'>$</td>
            </tr>
            <tr>
                <td>
                    <div align='center'>
                        <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            <br>
                        </font>
                    </div>
                </td>
                <td colspan='3'>&nbsp;</td>
                <td class='LETRAPEQUE&Ntilde;A'>$</td>
            </tr>

            <tr>
                <td colspan='4'><div align='right'><font size='1'>TOTAL</font></div></td>
                <td>
                    <div align='right'>
                        <font size='2' face='Verdana, Arial, Helvetica, sans-serif'>
                            
                            <strong>{7}</strong>
                        </font>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan='5'>
                    <div align='center' class='LETRAPEQUE&Ntilde;A'>
                        <font size='2' face='Verdana, Arial, Helvetica, sans-serif'>
                            OBSERVACIONES:
                            Pague en efectivo, cheque de gerencia o persona jur&iacute;dica LOCAL, en las oficinas de los bancos autorizados en cualquier ciudad del Pa&iacute;s. Si paga en cheque, por favor girar a nombre de ICETEX, escriba en el respaldo su nombre, numero de identificaci&oacute;n, concepto de la consignaci&oacute;n y tel&eacute;fonos
                        </font>
                    </div>
                </td>
            </tr>
        </table>
        <table width='650' border='0' cellspacing='0' cellpadding='0'>
            <tr>
                <td>
                    <div align='center' style='margin:5px'>
                        <div id='ImagenBarras'><img style='-webkit-user-select: none;' src='{8}' /></div>
                        <!-- <img style='-webkit-user-select: none;' src='http://10.1.4.241/librerias/GenerarCodigoBarras/imagenes/(415)7707268760020(8020)9993780287(3900)20000(96)20180606.gif'> -->
                        <!-- <img style='-webkit-user-select: none;' src='http://10.1.4.241/librerias/GenerarCodigoBarras/imagenes/(415)7707268760020(8020)9993780284(3900)1000000(96)20180606.gif'> -->


                    </div>
                    <div style='text-align:center; font-size:0.7em; padding:2px 0px 5px 0px;'><span id='TextoCodigoBarras'>{9}</span></div>
                </td>
            </tr>
        </table>
        <img src='http://10.1.18.120/ApiRecibo/Content/Cortar.jpg' width='650' height='21'>

        <br>


    </div>

 <div align='center'>
        <table width='100%' border='0' cellspacing='0'>
            <tr>
                <td>
                    <p align='center'>
                        <font size='2' face='Verdana, Arial, Helvetica, sans-serif'>
                            <strong>FONDO ESPECIAL BECAS SER </strong><br>
                            Artículo 104 Ley 1819 de 2016 <br> Decreto XXXX de 2018 / Resolución XXX de 2018<br>
                        </font>
                </td>
            </tr>
        </table>
        <br>

        <table width='649' border='1' cellpadding='0' cellspacing='0' bordercolor='#000000'>
            <tr>
                <td colspan='2' rowspan='4'>
                    <div align='center'>
                        <img src='http://10.1.18.120/ApiRecibo/Content/LOGO.JPG' width='155' height='38'><br>
                        <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            NIT 899.999.035-7<br>
                            Recibo de Pago No. {10}

                        </font>
                    </div>
                </td>
                <td width='98' class='LETRAPEQUE&Ntilde;A'>Razon Social</td>
                <td width='170' class='LETRAPEQUE&Ntilde;A'><font size='2' face='Verdana, Arial, Helvetica, sans-serif'>{0}</font></td>
                <td width='201' rowspan='5' valign='top'>
                    <div align='center'>
                        <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            TIMBRE CAJERO<br>(Copia Depositante)<br>
                        </font>
                    </div>
                </td>
            </tr>
            <tr>
                <td class='LETRAPEQUE&Ntilde;A'>Direcci&oacute;n</td>
                <td class='LETRAPEQUE&Ntilde;A'>{1}</td>
            </tr>
            <tr>
                <td class='LETRAPEQUE&Ntilde;A'>Tel&eacute;fonos</td>
                <td class='LETRAPEQUE&Ntilde;A'>{2}</td>
            </tr>
            <tr>
                <td class='LETRAPEQUE&Ntilde;A'>Ciudad</td>
                <td class='LETRAPEQUE&Ntilde;A'>{3}</td>
            </tr>
            <tr>
                <td height='53' colspan='4'>
                    <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                        Concepto de Este Pago
                    </font>:<span class=''>
                        <font size='2' face='Verdana, Arial, Helvetica, sans-serif'> Donacion - {0} - Programa: {4}, Valor: <strong>{7}</strong></font>
                    </span>
                </td>
            </tr>
            <tr>
                <td colspan='5' bgcolor='#999999'>
                    <div align='center'>
                        <font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            <strong>
                                ENTIDADES
                                BANCARIAS AUTORIZADAS
                            </strong>
                        </font>
                    </div>
                </td>
            </tr>
            <tr class=''>

                <td colspan='2'> <div align='center'><font size='2' face='Verdana, Arial, Helvetica, sans-serif'>BANCOLOMBIA - BANCO AGRARIO </font></div></td>
                <td colspan='2'> <div align='center'><font size='2' face='Verdana, Arial, Helvetica, sans-serif'>BBVA - BCSC CAJA SOCIAL </font></div></td>
                <td> <div align='center'><font size='2' face='Verdana, Arial, Helvetica, sans-serif'>HELM BANK - POPULAR - OCCIDENTE</font></div></td>
            </tr>
            <tr>
                <td colspan='5' bgcolor='#999999'>
                    <div align='center'><font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'></font></div>
                    <div align='center'>
                        <font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            <strong>
                                DETALLE
                                DEL PAGO
                            </strong>
                        </font>
                    </div>
                </td>
            </tr>
            <tr>
                <td width='154' height='40' valign='top'>
                    <div align='center'>
                        <strong>
                            <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                                PAGO
                                OPORTUNO HASTA<br>
                                {5}
                            </font>
                        </strong>
                    </div>
                </td>
                <td height='40' colspan='2' valign='top'>
                    <div align='center'>
                        <strong>
                            <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                                FECHA
                                DE PAGO<br>
                                aaaa/mm/dd
                            </font>
                        </strong>
                    </div>
                </td>
                <td colspan='2' valign='middle'>
                    <div align='center'>
                        <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            <strong>
                                C&oacute;digo
                                Referencia<br>
                                <font size='2'> </font>
                            </strong>
                        </font><font size='2' face='Verdana, Arial, Helvetica, sans-serif'>
                            Fondo Becaser {6}
                        </font>
                    </div>
                </td>
            </tr>

            <tr>
                <td bgcolor='#999999'>
                    <div align='center'>
                        <strong>
                            <font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                                C&oacute;digo
                                Banco
                            </font>
                        </strong>
                    </div>
                </td>
                <td colspan='3' bgcolor='#999999'>
                    <div align='center'>
                        <strong>
                            <font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                                Cheque
                                de Gerencia No.
                            </font>
                        </strong>
                    </div>
                    <div align='center'><strong></strong></div>
                </td>
                <td bgcolor='#999999'> <div align='center'><strong><font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>Valor</font></strong></div></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan='3'>&nbsp;</td>
                <td class='LETRAPEQUE&Ntilde;A'>$</td>
            </tr>
            <tr>
                <td>
                    <div align='center'>
                        <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
                            <br>
                        </font>
                    </div>
                </td>
                <td colspan='3'>&nbsp;</td>
                <td class='LETRAPEQUE&Ntilde;A'>$</td>
            </tr>

            <tr>
                <td colspan='4'><div align='right'><font size='1'>TOTAL</font></div></td>
                <td>
                    <div align='right'>
                        <font size='2' face='Verdana, Arial, Helvetica, sans-serif'>
                            
                            <strong>{7}</strong>
                        </font>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan='5'>
                    <div align='center' class='LETRAPEQUE&Ntilde;A'>
                        <font size='2' face='Verdana, Arial, Helvetica, sans-serif'>
                            OBSERVACIONES:
                            Pague en efectivo, cheque de gerencia o persona jur&iacute;dica LOCAL, en las oficinas de los bancos autorizados en cualquier ciudad del Pa&iacute;s. Si paga en cheque, por favor girar a nombre de ICETEX, escriba en el respaldo su nombre, numero de identificaci&oacute;n, concepto de la consignaci&oacute;n y tel&eacute;fonos
                        </font>
                    </div>
                </td>
            </tr>
        </table>
        <table width='650' border='0' cellspacing='0' cellpadding='0'>
            <tr>
                <td>
                    <div align='center' style='margin:5px'>
                        <div id='ImagenBarras'><img style='-webkit-user-select: none;' src='{8}' /></div>
                        <!-- <img style='-webkit-user-select: none;' src='http://10.1.4.241/librerias/GenerarCodigoBarras/imagenes/(415)7707268760020(8020)9993780287(3900)20000(96)20180606.gif'> -->
                        <!-- <img style='-webkit-user-select: none;' src='http://10.1.4.241/librerias/GenerarCodigoBarras/imagenes/(415)7707268760020(8020)9993780284(3900)1000000(96)20180606.gif'> -->


                    </div>
                    <div style='text-align:center; font-size:0.7em; padding:2px 0px 5px 0px;'><span id='TextoCodigoBarras'>{9}</span></div>
                </td>
            </tr>
        </table>
        <img src='http://10.1.18.120/ApiRecibo/Content/Cortar.jpg' width='650' height='21'>

        <br>


    </div></div>", modelo.RAZON_SOCIAL, modelo.DIRECCION, modelo.TELEFONO, modelo.CIUDAD, modelo.CONCEPTO, modelo.PAGO_OPORTUNO, modelo.CODIGO_REFERENCIA, string.Format("{0:C}", Convert.ToDecimal(modelo.TOTAL)), modelo.CODIGO_BARRAS,modelo.TEXTO_CODIGOBARRAS,modelo.NORECIBO);
                    respuesta = htmlpdf;
                    
                    


                }
                else
                {
                    respuesta = "Usuario No Valido verifique sus credenciales";
                }
            }
            catch (Exception ex)
            {
                respuesta = "¡La URL no contiene el formato de encriptacion correcto!...";
                
            }
            
            return respuesta;
        }


    //    public string GetReciboSC(string dd="")
    //    {
    //        string respuesta = "";
    //        try
    //        {
    //            var Url = HttpContext.Current.Request.RawUrl;
    //            string _iacx = ""; string _referenciax = ""; string _valorx = ""; string _fechax = ""; string _usuario_wa = ""; string _password_wa = "";
    //            int _idpersona = 0; int _idproducto = 1003;
    //            var urldecode = Url.Substring(Url.LastIndexOf("?") + 1);
    //            urldecode = Encryption.RSADecrypt(urldecode);

    //            var _parameters = JsonConvert.DeserializeObject<WARecibo>(urldecode);

    //            //var _parameters = new WARecibo { iacx = _iacx, referenciax = _referenciax, valorx = _valorx, fechax = _fechax, idpersona = _idpersona, idproducto = _idproducto, usuario_wa = _usuario_wa, password_wa = _password_wa };
    //            //var _params = JsonConvert.SerializeObject(_parameters);
    //            //var hghgh = Encryption.RSAEncrypt(_params);
    //            //var _bytes = Encoding.UTF8.GetByteCount(_params);
    //            //var ser = Encryption.RSAEncrypt(_params);

    //            //WARecibo Parameters = (WARecibo)JsonConvert.DeserializeObject(_parameters);
    //            //HttpContext.Current.Request. = Url + urlEncrypt;
    //            Logica.ReciboLN reciboLN = new Logica.ReciboLN();
    //            UsuarioWA usr = new UsuarioWA();
    //            usr.IDPRODUCTO = _parameters.idproducto;
    //            usr.USUARIO_WA = _parameters.usuario_wa;
    //            usr.PASSWORD_WA = _parameters.password_wa;
    //            usr = reciboLN.Validar(usr);

    //            if (usr.VALIDO)
    //            {
    //                respuesta = "Generado";
    //                ReciboUnico.Controllers.ReciboController Rcontrol = new ReciboUnico.Controllers.ReciboController();
    //                ViewResult Resultado = (ViewResult)Rcontrol.ReciboUnico(_parameters.iacx, _parameters.referenciax, _parameters.valorx, _parameters.fechax, _parameters.idpersona);
    //                Entidades.Recibo modelo = (Entidades.Recibo)Resultado.ViewData.Model;
    //                string path = HttpContext.Current.Server.MapPath(@"~/Views/Recibo/");
    //                System.Net.WebClient wc = new System.Net.WebClient();
    //                string Urlcombi = "http://10.1.4.241/librerias/GenerarCodigoBarras/Default.aspx?IAC=" + _parameters.iacx + "&REFERENCIA=" + _parameters.referenciax + "&VALOR=" + _parameters.valorx + "&FECHA=" + _parameters.fechax;
    //                WebRequest request = WebRequest.Create(Urlcombi);
    //                WebResponse response = request.GetResponse();
    //                StreamReader reader = new StreamReader(response.GetResponseStream());
    //                string str = reader.ReadLine();
    //                string htmlpdf = string.Format(@"<div id='ContenedorRecibo'><div align='center'>
    //    <table width= '100%' border= '0' cellspacing= '0'>
    //             <tbody><tr>
    //                 <td>
    //                     <p align= 'center'>
    //                          <strong> FORMULARIO DE REGISTRO DE APORTES<br>Fondo Becaser</strong><br>
    //                             FONDO COMUN DE EDUCACION FORMAL<br>Artículo 8 Ley 863 de 2003 <br> Decreto 2880 de 2004 / Resolución 051 de 2005 <br>
    //                       </p></td>
    //                   </tr>
    //               </tbody></table>
    //               <br>
    //               <table width= '649' border= '1' cellpadding= '0' cellspacing= '0' bordercolor= '#000000'>
    //                            <tbody><tr>
    //                                <td colspan= '2' rowspan= '4'>
    //                                       <div align= 'center'>
    //                                            <img src= 'http://10.1.18.120:7000/facturacion/IMAGENES/LOGO.JPG' width= '155' height= '38'><br>
    //                                                 <font size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                                                        NIT 899.999.035 - 7 <br>
    //                                                        Recibo de Pago No. 8727
    //                                                    </font>
    //                                                </div>
    //                                            </td>
    //                                            <td width= '98' class='LETRAPEQUEÑA'>Razon Social</td>
    //            <td width= '170' class='LETRAPEQUEÑA'>{0}</td>
    //            <td width= '201' rowspan= '5' valign= 'top'>
    //                  <div align= 'center'>
    //                      <font size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                          TIMBRE CAJERO<br>(Copia Depositante)<br>
    //                    </font>
    //                </div>
    //            </td>
    //        </tr>
    //        <tr>
    //            <td class='LETRAPEQUEÑA'>Dirección</td>
    //            <td class='LETRAPEQUEÑA'>{1}</td>
    //        </tr>
    //        <tr>
    //            <td class='LETRAPEQUEÑA'>Teléfonos</td>
    //            <td class='LETRAPEQUEÑA'>{2}</td>
    //        </tr>
    //        <tr>
    //            <td class='LETRAPEQUEÑA'>Ciudad</td>
    //            <td class='LETRAPEQUEÑA'> {3}</td>
    //        </tr>
    //        <tr>
    //            <td height= '53' colspan='4'>
    //                <font size= '1' face='Verdana, Arial, Helvetica, sans-serif'>
    //                    Concepto de Este Pago
    //                </font>:<span class='LETRAPEQUEÑA'>
    //                    DONACION PARA Fondo Becaser VALOR:{4}
    //                </span>
    //            </td>
    //        </tr>
    //        <tr>
    //            <td colspan= '5' bgcolor='#999999'>
    //                <div align= 'center'>
    //                    <font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>
    //                        <strong>
    //                            ENTIDADES
    //                            BANCARIAS AUTORIZADAS
    //                        </strong>
    //                    </font>
    //                </div>
    //            </td>
    //        </tr>
    //        <tr class='LETRAPEQUEÑA'>
    //            <td colspan= '2'> <div align='center'>BANCOLOMBIA - BANCO AGRARIO</div></td>
    //            <td colspan= '2'> <div align= 'center'> BBVA - BCSC CAJA SOCIAL </div></td>
    //            <td> <div align= 'center'> HELM BANK - POPULAR - OCCIDENTE</div></td>
    //        </tr>
    //        <tr>
    //            <td colspan= '5' bgcolor= '#999999'>
    //                    <div align= 'center'><font color= '#FFFFFF' size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'></font></div>
    //                    <div align= 'center'>
    //                        <font color= '#FFFFFF' size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                            <strong>
    //                                DETALLE DEL PAGO
    //                        </strong>
    //                    </font>
    //                </div>
    //            </td>
    //        </tr>
    //        <tr>
    //            <td width= '154' height= '40' valign= 'top'>
    //                    <div align= 'center'>
    //                        <strong>
    //                            <font size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                                PAGO
    //                                OPORTUNO HASTA<br>
    //                            {5}
    //                        </font>
    //                    </strong>
    //                </div>
    //            </td>
    //            <td height= '40' colspan= '2' valign= 'top'>
    //                    <div align= 'center'>
    //                        <strong>
    //                            <font size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                                FECHA DE PAGO<br>
    //                                aaaa/mm/dd
    //                            </font>
    //                        </strong>
    //                </div>
    //            </td>
    //            <td colspan= '2' valign= 'middle'>
    //                    <div align= 'center'>
    //                        <font size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                            <strong>
    //                                Código
    //                                Referencia<br>
    //                                <font size= '2'> </font>
    //                            </strong>
    //                        </font><font size= '2' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                            Fondo Becaser  {6}
    //                    </font>
    //                </div>
    //            </td>
    //        </tr>
    //        <tr>
    //            <td bgcolor= '#999999'>
    //                    <div align= 'center'>
    //                        <strong>
    //                            <font color= '#FFFFFF' size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                                Código
    //                                Banco
    //                            </font>
    //                        </strong>
    //                </div>
    //            </td>
    //            <td colspan= '3' bgcolor= '#999999'>
    //                    <div align= 'center'>
    //                        <strong>
    //                            <font color= '#FFFFFF' size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                                Cheque
    //                                de Gerencia No.
    //                            </font>
    //                    </strong>
    //                </div>
    //                <div align= 'center'><strong></strong></div>
    //                </td>
    //                <td bgcolor= '#999999'> <div align= 'center'><strong><font color= '#FFFFFF' size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'> Valor </font></strong></div></td>
    //            </tr>
    //            <tr>
    //                <td> &nbsp;</td>
    //            <td colspan= '3'> &nbsp;</td>
    //            <td class='LETRAPEQUEÑA'>$</td>
    //        </tr>
    //        <tr>
    //            <td>
    //                <div align= 'center'>
    //                    <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
    //                        <br>
    //                    </font>
    //                </div>
    //            </td>
    //            <td colspan= '3'> &nbsp;</td>
    //            <td class='LETRAPEQUEÑA'>$</td>
    //        </tr>
    //        <tr>
    //            <td colspan= '4'><div align='right'><font size= '1'> TOTAL </font></div></td>
    //                <td>
    //                    <div align='right'>
    //                    <font size= '1' face='Verdana, Arial, Helvetica, sans-serif'>$</font>
    //                    <font size= '1' face='Verdana, Arial, Helvetica, sans-serif'>
    //                        {7}
    //                    </font>
    //                </div>
    //            </td>
    //        </tr>
    //        <tr>
    //            <td colspan= '5'>
    //                <div align='center' class='LETRAPEQUEÑA'>
    //                    OBSERVACIONES:
    //                    Pague en efectivo, cheque de gerencia o persona jurídica LOCAL, en las oficinas de los bancos autorizados en cualquier ciudad del País.Si paga en cheque, por favor girar a nombre de ICETEX, escriba en el respaldo su nombre, numero de identificación, concepto de la consignación y teléfonos
    //                </div>
    //            </td>
    //        </tr>
    //    </tbody></table>
    //    <table width= '650' border= '0' cellspacing= '0' cellpadding= '0'>
    //        <tbody><tr>
    //            <td>
    //                <div align= 'center' style= 'margin:5px'>
    //                    <div id= 'ImagenBarras'><img style= '-webkit-user-select: none;' src= '{8}'></div>
    //                </div>
    //                <div style= 'text-align:center; font-size:0.7em; padding:2px 0px 5px 0px;'><span id= 'TextoCodigoBarras'> (415)7707268760020(8020)9993780287(3900)38250000(96)20180508</span></div>
    //            </td>
    //        </tr>
    //    </tbody></table>
    //    <img src= 'http://10.1.18.120:7000/facturacion/IMAGENES/CORTAR.JPG' width='650' height='21'>
    //    <br>
    //</div>
    //<div align='center'>
    //    <table width= '100%' border= '0' cellspacing= '0'>
    //             <tbody><tr>
    //                 <td>
    //                     <p align= 'center'>
    //                          <strong> FORMULARIO DE REGISTRO DE APORTES<br>Fondo Becaser</strong><br>
    //                             FONDO COMUN DE EDUCACION FORMAL<br>Artículo 8 Ley 863 de 2003 <br> Decreto 2880 de 2004 / Resolución 051 de 2005 <br>
    //                       </p></td>
    //                   </tr>
    //               </tbody></table>
    //               <br>
    //               <table width= '649' border= '1' cellpadding= '0' cellspacing= '0' bordercolor= '#000000'>
    //                            <tbody><tr>
    //                                <td colspan= '2' rowspan= '4'>
    //                                       <div align= 'center'>
    //                                            <img src= 'http://10.1.18.120:7000/facturacion/IMAGENES/LOGO.JPG' width= '155' height= '38'><br>
    //                                                 <font size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                                                        NIT 899.999.035 - 7 <br>
    //                                                        Recibo de Pago No. 8727
    //                                                    </font>
    //                                                </div>
    //                                            </td>
    //                                            <td width= '98' class='LETRAPEQUEÑA'>Razon Social</td>
    //            <td width= '170' class='LETRAPEQUEÑA'>{0}</td>
    //            <td width= '201' rowspan= '5' valign= 'top'>
    //                  <div align= 'center'>
    //                      <font size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                          TIMBRE CAJERO<br>(Copia Depositante)<br>
    //                    </font>
    //                </div>
    //            </td>
    //        </tr>
    //        <tr>
    //            <td class='LETRAPEQUEÑA'>Dirección</td>
    //            <td class='LETRAPEQUEÑA'>{1}</td>
    //        </tr>
    //        <tr>
    //            <td class='LETRAPEQUEÑA'>Teléfonos</td>
    //            <td class='LETRAPEQUEÑA'>{2}</td>
    //        </tr>
    //        <tr>
    //            <td class='LETRAPEQUEÑA'>Ciudad</td>
    //            <td class='LETRAPEQUEÑA'> {3}</td>
    //        </tr>
    //        <tr>
    //            <td height= '53' colspan='4'>
    //                <font size= '1' face='Verdana, Arial, Helvetica, sans-serif'>
    //                    Concepto de Este Pago
    //                </font>:<span class='LETRAPEQUEÑA'>
    //                    DONACION PARA Fondo Becaser VALOR:{4}
    //                </span>
    //            </td>
    //        </tr>
    //        <tr>
    //            <td colspan= '5' bgcolor='#999999'>
    //                <div align= 'center'>
    //                    <font color='#FFFFFF' size='1' face='Verdana, Arial, Helvetica, sans-serif'>
    //                        <strong>
    //                            ENTIDADES
    //                            BANCARIAS AUTORIZADAS
    //                        </strong>
    //                    </font>
    //                </div>
    //            </td>
    //        </tr>
    //        <tr class='LETRAPEQUEÑA'>
    //            <td colspan= '2'> <div align='center'>BANCOLOMBIA - BANCO AGRARIO</div></td>
    //            <td colspan= '2'> <div align= 'center'> BBVA - BCSC CAJA SOCIAL </div></td>
    //            <td> <div align= 'center'> HELM BANK - POPULAR - OCCIDENTE</div></td>
    //        </tr>
    //        <tr>
    //            <td colspan= '5' bgcolor= '#999999'>
    //                    <div align= 'center'><font color= '#FFFFFF' size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'></font></div>
    //                    <div align= 'center'>
    //                        <font color= '#FFFFFF' size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                            <strong>
    //                                DETALLE DEL PAGO
    //                        </strong>
    //                    </font>
    //                </div>
    //            </td>
    //        </tr>
    //        <tr>
    //            <td width= '154' height= '40' valign= 'top'>
    //                    <div align= 'center'>
    //                        <strong>
    //                            <font size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                                PAGO
    //                                OPORTUNO HASTA<br>
    //                            {5}
    //                        </font>
    //                    </strong>
    //                </div>
    //            </td>
    //            <td height= '40' colspan= '2' valign= 'top'>
    //                    <div align= 'center'>
    //                        <strong>
    //                            <font size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                                FECHA DE PAGO<br>
    //                                aaaa/mm/dd
    //                            </font>
    //                        </strong>
    //                </div>
    //            </td>
    //            <td colspan= '2' valign= 'middle'>
    //                    <div align= 'center'>
    //                        <font size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                            <strong>
    //                                Código
    //                                Referencia<br>
    //                                <font size= '2'> </font>
    //                            </strong>
    //                        </font><font size= '2' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                            Fondo Becaser  {6}
    //                    </font>
    //                </div>
    //            </td>
    //        </tr>
    //        <tr>
    //            <td bgcolor= '#999999'>
    //                    <div align= 'center'>
    //                        <strong>
    //                            <font color= '#FFFFFF' size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                                Código
    //                                Banco
    //                            </font>
    //                        </strong>
    //                </div>
    //            </td>
    //            <td colspan= '3' bgcolor= '#999999'>
    //                    <div align= 'center'>
    //                        <strong>
    //                            <font color= '#FFFFFF' size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'>
    //                                Cheque
    //                                de Gerencia No.
    //                            </font>
    //                    </strong>
    //                </div>
    //                <div align= 'center'><strong></strong></div>
    //                </td>
    //                <td bgcolor= '#999999'> <div align= 'center'><strong><font color= '#FFFFFF' size= '1' face= 'Verdana, Arial, Helvetica, sans-serif'> Valor </font></strong></div></td>
    //            </tr>
    //            <tr>
    //                <td> &nbsp;</td>
    //            <td colspan= '3'> &nbsp;</td>
    //            <td class='LETRAPEQUEÑA'>$</td>
    //        </tr>
    //        <tr>
    //            <td>
    //                <div align= 'center'>
    //                    <font size='1' face='Verdana, Arial, Helvetica, sans-serif'>
    //                        <br>
    //                    </font>
    //                </div>
    //            </td>
    //            <td colspan= '3'> &nbsp;</td>
    //            <td class='LETRAPEQUEÑA'>$</td>
    //        </tr>
    //        <tr>
    //            <td colspan= '4'><div align='right'><font size= '1'> TOTAL </font></div></td>
    //                <td>
    //                    <div align='right'>
    //                    <font size= '1' face='Verdana, Arial, Helvetica, sans-serif'>$</font>
    //                    <font size= '1' face='Verdana, Arial, Helvetica, sans-serif'>
    //                        {7}
    //                    </font>
    //                </div>
    //            </td>
    //        </tr>
    //        <tr>
    //            <td colspan= '5'>
    //                <div align='center' class='LETRAPEQUEÑA'>
    //                    OBSERVACIONES:
    //                    Pague en efectivo, cheque de gerencia o persona jurídica LOCAL, en las oficinas de los bancos autorizados en cualquier ciudad del País.Si paga en cheque, por favor girar a nombre de ICETEX, escriba en el respaldo su nombre, numero de identificación, concepto de la consignación y teléfonos
    //                </div>
    //            </td>
    //        </tr>
    //    </tbody></table>
    //    <table width= '650' border= '0' cellspacing= '0' cellpadding= '0'>
    //        <tbody><tr>
    //            <td>
    //                <div align= 'center' style= 'margin:5px'>
    //                    <div id= 'ImagenBarras'><img style= '-webkit-user-select: none;' src= '{8}'></div>
    //                </div>
    //                <div style= 'text-align:center; font-size:0.7em; padding:2px 0px 5px 0px;'><span id= 'TextoCodigoBarras'> (415)7707268760020(8020)9993780287(3900)38250000(96)20180508</span></div>
    //            </td>
    //        </tr>
    //    </tbody></table>
    //    <img src= 'http://10.1.18.120:7000/facturacion/IMAGENES/CORTAR.JPG' width='650' height='21'>
    //    <br>
    //</div></div>", modelo.RAZON_SOCIAL, modelo.DIRECCION, modelo.TELEFONO, modelo.CIUDAD, modelo.CONCEPTO, modelo.PAGO_OPORTUNO, modelo.CODIGO_REFERENCIA, modelo.TOTAL, modelo.CODIGO_BARRAS);
    //                respuesta = htmlpdf;



    //            }
    //            else
    //            {
    //                respuesta = "Usuario No Valido verifique sus credenciales";
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            respuesta = "¡La URL no contiene el formato de encriptacion correcto!...";

    //        }

    //        return respuesta;
    //    }
        public string GetEncryptUrl(string _valorx, string _fechax, int _idpersona,
            int _idproducto, string _usuario_wa, string _password_wa, int _tipopersona, string _IdProgramaBeca)
        {
            var _parameters = new WARecibo { valorx = _valorx, fechax = _fechax,
                idpersona = _idpersona, idproducto = _idproducto, usuario_wa = _usuario_wa, password_wa = _password_wa,tipopersona= _tipopersona,
                IdProgramaBeca = _IdProgramaBeca };
            var _paramsJson = JsonConvert.SerializeObject(_parameters);
            var _paramsEncrypt = Encryption.RSAEncrypt(_paramsJson);
            

            return _paramsEncrypt;
        }



        //public HttpresponseMessage Display(string docid)
        //{
        //    HttpresponseMessage response= Request.Createresponse(HttpStatusCode.BadRequest);
        //    var documents= reader.GetDocument(docid);
        //    if (documents != null && documents.Length == 1)
        //    {
        //        var document= documents[0];
        //        docid= document.docid;
        //        byte[] buffer= new byte[0];
        //        //generate pdf document
        //        MemoryStream memoryStream= new MemoryStream();
        //        //MyPDFGenerator.New().PrintToStream(document, memoryStream);
        //        BinaryFormatter bf= new BinaryFormatter();
        //        using (MemoryStream ms= new MemoryStream())
        //        {
        //            bf.Serialize(ms, documents);
        //            ms.ToArray();
        //            memoryStream= ms;
        //        }
        //        //get buffer
        //        buffer= memoryStream.ToArray();
        //        //content length for use in header
        //        var contentLength= buffer.Length;
        //        //200
        //        //successful
        //        var statuscode= HttpStatusCode.OK;
        //        response= Request.Createresponse(statuscode);
        //        response.Content= new StreamContent(new MemoryStream(buffer));
        //        response.Content.Headers.ContentType= new MediaTypeHeaderValue("application/pdf");
        //        response.Content.Headers.ContentLength= contentLength;
        //        ContentDispositionHeaderValue contentDisposition= null;
        //        if (ContentDispositionHeaderValue.TryParse("inline; filename=" + document.Name + ".pdf", out contentDisposition))
        //        {
        //            response.Content.Headers.ContentDisposition= contentDisposition;
        //        }
        //    }
        //    else
        //    {
        //        var statuscode= HttpStatusCode.NotFound;
        //        var message= String.Format("Unable to find resource. Resource \"{0}\" may not exist.", docid);
        //        //var responseData= responseDataFactory.CreateWithOnlyMetadata(statuscode, message);
        //        //response= Request.Createresponse((HttpStatusCode)responseData.meta.code, responseData);
        //    }
        //    return response;
        //}

        public HttpResponseMessage GetPdf(string Filename)
        {
            string fileName = Filename + ".pdf";
            var path = @"C:\Users\USER\Documents\Confidencial\Cuentas de cobro 2018\Enero\" + fileName;

            //check the directory for pdf matching the certid
            if (File.Exists(path))
            {
                //if there is a match then return the file
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(path, FileMode.Open);
                stream.Position = 0;
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = fileName };
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                result.Content.Headers.ContentDisposition.FileName = fileName;
                return result;
            }
            else
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.Gone);
                return result;
            }
        }


        //private void GetCertQueryresponse(string url, string serial)
        //{
        //    string encodedParameters= "certificateId=" + serial.Replace(" ", "");

        //    HttpWebRequest httpRequest= (HttpWebRequest)WebRequest.Create(url);

        //    httpRequest.Method= "POST";
        //    httpRequest.ContentType= "application/x-www-form-urlencoded";
        //    httpRequest.AllowAutoRedirect= false;

        //    byte[] bytedata= Encoding.UTF8.GetBytes(encodedParameters);
        //    httpRequest.ContentLength= bytedata.Length;

        //    Stream requestStream= httpRequest.GetRequestStream();
        //    requestStream.Write(bytedata, 0, bytedata.Length);
        //    requestStream.Close();

        //    HttpWebResponse response= (HttpWebResponse)httpRequest.GetResponse();

        //    if (response.StatusCode == HttpStatusCode.OK)
        //    {

        //        byte[] bytes= null;
        //        using (Stream stream= response.GetResponseStream())
        //        {


        //            using (MemoryStream ms= new MemoryStream())
        //            {
        //                int count= 0;
        //                do
        //                {
        //                    byte[] buf= new byte[1024];
        //                    count= stream.Read(buf, 0, 1024);
        //                    ms.Write(buf, 0, count);
        //                } while (stream.CanRead && count> 0);
        //                ms.Position= 0;
        //                bytes= ms.ToArray();
        //            }

        //            var filename= serial + ".pdf";


        //            response.Headers.Add("content-length", bytes.Length.ToString());
        //            response.Headers.Add("content-disposition", "inline; filename=MyFilename");
        //            response.Headers.Add("Expires", "0");
        //            response.Headers.Add("Pragma", "Cache");
        //            response.Headers.Add("Cache-Control", "private");

        //            response.
        //        }




        //    }
        //}
        public static string Encrypt_QueryString(string str)
        {
            string EncrptKey = "2013;[pnuLIT)WebCodeExpert";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

    }
    
}
