using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using XSDToXML.Utils;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
namespace XSDToXML
{
    class Program
    {
        //modifique por su path LEO
        static private string path = @"C:\Program Files (x86)\ekrontech\Facturacion3.3\";
        static private string pathl = @"\\\192.168.2.21\\b1_shr\\FAE\\ProcesoPagos\\";
        static string pathXML = pathl + @"PrimarioISI121121I31365.xml";
        static SqlConnection Cnx = null;
        static SqlDataAdapter sData = null;
        static DataTable dData = new DataTable();
        static DataTable dDataDetalle = new DataTable();
        static void Main(string[] args)
        {
            String sql; //LEO
            sql = "SELECT DISTINCT [LExpedicion],[TipoComprobante],[Total],[Moneda],[SubTotal],[Fecha],[Folio],[EmisorRegimenFiscal],[EmisorNombre],[EmisorRFC],[ReceptorNombre],[ReceptorRFC],[Importe],[ValorUnitario], [NArchivo],[Descripcion],[ClaveUnidad],[Cantidad],[ClaveProdServ],[Monto],[MonedaP],[FormaPagoP],[FechaPago],[TipoCambioPago] FROM [CERTIFICA].[dbo].[xPago] WHERE [Status] = 0"; //LEO
            Cnx = new SqlConnection("Data Source= 192.168.2.72;Initial Catalog=CERTIFICA;Persist Security Info=True;User ID=sa;password=*SaP5950*;Pooling=True;");
            try
            {
                Cnx.Open();

                sData = new SqlDataAdapter(sql, Cnx);
                sData.Fill(dData);
                foreach (DataRow item in dData.Rows)
                {
                    CPago(item);
                }
                Cnx.Close();
            }
            catch (Exception)
            {

                throw;
            }
            //CFact();
            //CPago();
        }

        private static void CFact()
        {

            //Obtener numero certificado------------------------------------------------------------


            string pathCer = pathl + @"TRCM\TRCM.cer";
            string pathKey = pathl + @"TRCM\TRCM.key";
            string clavePrivada = "sellorcm970923";

            //Obtenemos el numero
            string numeroCertificado, aa, b, c;
            SelloDigital.leerCER(pathCer, out aa, out b, out c, out numeroCertificado);


            //Llenamos la clase COMPROBANTE--------------------------------------------------------
            Comprobante oComprobante = new Comprobante();
            oComprobante.Version = "3.3";
            oComprobante.Serie = "Matriz";
            oComprobante.Folio = "1100130";
            oComprobante.Fecha = "2019-05-07T11:26:29";
            // oComprobante.Sello = "faltante"; //sig video
            oComprobante.FormaPago = "99";
            oComprobante.NoCertificado = numeroCertificado;
            // oComprobante.Certificado = ""; //sig video
            oComprobante.SubTotal = 3446.56m;
            oComprobante.Descuento = 0.00m;
            oComprobante.Moneda = "MXN";
            oComprobante.Total = 3998.00m;
            oComprobante.TipoDeComprobante = "I";
            oComprobante.MetodoPago = "PPD";
            oComprobante.LugarExpedicion = "64800";



            ComprobanteEmisor oEmisor = new ComprobanteEmisor();

            oEmisor.Rfc = "RCM970923SD1";
            oEmisor.Nombre = "The Right Connection México S.A. de C.V.";
            oEmisor.RegimenFiscal = "601";

            ComprobanteReceptor oReceptor = new ComprobanteReceptor();
            oReceptor.Nombre = "PLAZA AUTOMOTORES SA DE CV";
            oReceptor.Rfc = "PAU950814SR5";
            oReceptor.UsoCFDI = "G03";

            //asigno emisor y receptor
            oComprobante.Emisor = oEmisor;
            oComprobante.Receptor = oReceptor;

            // Inicio Concepto 1 
            List<ComprobanteConcepto> lstConceptos = new List<ComprobanteConcepto>();
            ComprobanteConcepto oConcepto = new ComprobanteConcepto();
            oConcepto.Importe = 1023.28m;
            oConcepto.ClaveProdServ = "01010101";
            oConcepto.Cantidad = 1;
            oConcepto.ClaveUnidad = "E48";
            oConcepto.Descripcion = "POLARIZADO COMP AUTOS Y TIPO ORIG MINI VANS";
            oConcepto.ValorUnitario = 1023.28m;
            oConcepto.Descuento = 0.00m;

            //lstConceptos.Add(oConcepto);
            //oComprobante.Conceptos = lstConceptos.ToArray();

            //Linea de Impuestos
            List<ComprobanteConceptoImpuestosTraslado> lstTraslado = new List<ComprobanteConceptoImpuestosTraslado>();
            ComprobanteConceptoImpuestosTraslado oTraslado = new ComprobanteConceptoImpuestosTraslado();
            oTraslado = new ComprobanteConceptoImpuestosTraslado();
            oTraslado.Importe = 163.72m;
            oTraslado.TasaOCuota = 0.160000m;
            oTraslado.Base = 1023.28m;
            oTraslado.TipoFactor = "Tasa";
            oTraslado.Impuesto = "002";


            lstTraslado.Add(oTraslado);
            lstConceptos.Add(oConcepto);

            oConcepto.Impuestos = new ComprobanteConceptoImpuestos();

            oComprobante.Conceptos = lstConceptos.ToArray();
            oConcepto.Impuestos.Traslados = lstTraslado.ToArray();
            //Fin Concepto 1
            // Inicio Concepto 2 

            oConcepto = new ComprobanteConcepto();
            oConcepto.Importe = 1023.28m;
            oConcepto.ClaveProdServ = "01010101";
            oConcepto.Cantidad = 1;
            oConcepto.ClaveUnidad = "H87";
            oConcepto.Descripcion = "POLARIZADO INTELIGENTE 2 VENTANAS";
            oConcepto.ValorUnitario = 1023.28m;
            oConcepto.Descuento = 0.00m;

            //2stConceptos.Add(oConcepto);
            //oComprobante.Conceptos = 2stConceptos.ToArray();

            //Linea de Impuestos
            List<ComprobanteConceptoImpuestosTraslado> lstTraslado2 = new List<ComprobanteConceptoImpuestosTraslado>();
            oTraslado = new ComprobanteConceptoImpuestosTraslado();
            oTraslado.Importe = 163.72m;
            oTraslado.TasaOCuota = 0.160000m;
            oTraslado.Base = 1023.28m;
            oTraslado.TipoFactor = "Tasa";
            oTraslado.Impuesto = "002";


            lstTraslado2.Add(oTraslado);
            lstConceptos.Add(oConcepto);

            oConcepto.Impuestos = new ComprobanteConceptoImpuestos();

            oComprobante.Conceptos = lstConceptos.ToArray();
            oConcepto.Impuestos.Traslados = lstTraslado2.ToArray();
            //Fin Concepto 2  
            //Inicio Concepto 3

            oConcepto = new ComprobanteConcepto();
            oConcepto.Importe = 1400.00m;
            oConcepto.ClaveProdServ = "41111926";
            oConcepto.Cantidad = 1;
            oConcepto.ClaveUnidad = "H87";
            oConcepto.Descripcion = "SENSORES DE PROXIMIDAD NEGRO";
            oConcepto.ValorUnitario = 1400.00m;
            oConcepto.Descuento = 0.00m;

            //2stConceptos.Add(oConcepto);
            //oComprobante.Conceptos = 2stConceptos.ToArray();

            //Linea de Impuestos
            List<ComprobanteConceptoImpuestosTraslado> lstTraslado3 = new List<ComprobanteConceptoImpuestosTraslado>(); //Cambio
            oTraslado = new ComprobanteConceptoImpuestosTraslado();
            oTraslado.Importe = 224.00m;
            oTraslado.TasaOCuota = 0.160000m;
            oTraslado.Base = 1400.00m;
            oTraslado.TipoFactor = "Tasa";
            oTraslado.Impuesto = "002";


            lstTraslado3.Add(oTraslado); //Cambio
            lstConceptos.Add(oConcepto);

            oConcepto.Impuestos = new ComprobanteConceptoImpuestos();

            oComprobante.Conceptos = lstConceptos.ToArray();
            oConcepto.Impuestos.Traslados = lstTraslado3.ToArray();//Cambio

            //Fin Concepto 3
            ////Inicio Concepto 4

            //oConcepto = new ComprobanteConcepto();
            //oConcepto.Importe = 125.19m;
            //oConcepto.ClaveProdServ = "39122300";
            //oConcepto.Cantidad = 3;
            //oConcepto.ClaveUnidad = "H87";
            //oConcepto.Descripcion = "RELAY DE 40/30 AMPS";
            //oConcepto.ValorUnitario = 41.73m;
            //oConcepto.Descuento = 0.00m;

            //2stConceptos.Add(oConcepto);
            //oComprobante.Conceptos = 2stConceptos.ToArray();

            //Linea de Impuestos
            //List<ComprobanteConceptoImpuestosTraslado> lstTraslado4 = new List<ComprobanteConceptoImpuestosTraslado>(); //Cambio
            //oTraslado = new ComprobanteConceptoImpuestosTraslado();
            //oTraslado.Importe = 20.03m;
            //oTraslado.TasaOCuota = 0.160000m;
            //oTraslado.Base = 125.19m;
            //oTraslado.TipoFactor = "Tasa";
            //oTraslado.Impuesto = "002";


            //lstTraslado4.Add(oTraslado); //Cambio
            //lstConceptos.Add(oConcepto);

            //oConcepto.Impuestos = new ComprobanteConceptoImpuestos();

            //oComprobante.Conceptos = lstConceptos.ToArray();
            //oConcepto.Impuestos.Traslados = lstTraslado4.ToArray();//Cambio

            //Fin Concepto 4
            ////Inicio Concepto 5

            //oConcepto = new ComprobanteConcepto();
            //oConcepto.Importe = 20.00m;
            //oConcepto.ClaveProdServ = "01010101";
            //oConcepto.Cantidad = 1;
            //oConcepto.ClaveUnidad = "H87";
            //oConcepto.Descripcion = "SUPER SWITCH AJUSTABLE";
            //oConcepto.ValorUnitario = 20.00m;
            //oConcepto.Descuento = 0.00m;

            //2stConceptos.Add(oConcepto);
            //oComprobante.Conceptos = 2stConceptos.ToArray();

            //Linea de Impuestos
            //List<ComprobanteConceptoImpuestosTraslado> lstTraslado5 = new List<ComprobanteConceptoImpuestosTraslado>(); //Cambio
            //oTraslado = new ComprobanteConceptoImpuestosTraslado();
            //oTraslado.Importe = 3.20m;
            //oTraslado.TasaOCuota = 0.160000m;
            //oTraslado.Base = 20.00m;
            //oTraslado.TipoFactor = "Tasa";
            //oTraslado.Impuesto = "002";


            //lstTraslado5.Add(oTraslado); //Cambio
            //lstConceptos.Add(oConcepto);

            //oConcepto.Impuestos = new ComprobanteConceptoImpuestos();

            //oComprobante.Conceptos = lstConceptos.ToArray();
            //oConcepto.Impuestos.Traslados = lstTraslado5.ToArray();//Cambio

            //Fin Concepto 5
            ////Inicio Concepto 6

            //oConcepto = new ComprobanteConcepto();
            //oConcepto.Importe = 128.40m;
            //oConcepto.ClaveProdServ = "01010101";
            //oConcepto.Cantidad = 1;
            //oConcepto.ClaveUnidad = "H87";
            //oConcepto.Descripcion = "SWITCH PARA SEGUROS ELECTRICOS";
            //oConcepto.ValorUnitario = 128.40m;
            //oConcepto.Descuento = 0.00m;

            //2stConceptos.Add(oConcepto);
            //oComprobante.Conceptos = 2stConceptos.ToArray();

            //Linea de Impuestos
            //List<ComprobanteConceptoImpuestosTraslado> lstTraslado6 = new List<ComprobanteConceptoImpuestosTraslado>(); //Cambio
            //oTraslado = new ComprobanteConceptoImpuestosTraslado();
            //oTraslado.Importe = 20.54m;
            //oTraslado.TasaOCuota = 0.160000m;
            //oTraslado.Base = 128.40m;
            //oTraslado.TipoFactor = "Tasa";
            //oTraslado.Impuesto = "002";


            //lstTraslado6.Add(oTraslado); //Cambio
            //lstConceptos.Add(oConcepto);

            //oConcepto.Impuestos = new ComprobanteConceptoImpuestos();

            //oComprobante.Conceptos = lstConceptos.ToArray();
            //oConcepto.Impuestos.Traslados = lstTraslado6.ToArray();//Cambio

            //Fin Concepto 6
            ////Inicio Concepto 7

            //oConcepto = new ComprobanteConcepto();
            //oConcepto.Importe = 466.40m;
            //oConcepto.ClaveProdServ = "01010101";
            //oConcepto.Cantidad = 500;
            //oConcepto.ClaveUnidad = "H87";
            //oConcepto.Descripcion = "ZAPATA FORRADA HEMBRA AZUL 16-14 .250 OVAL";
            //oConcepto.ValorUnitario = 0.93m;
            //oConcepto.Descuento = 0.00m;

            //2stConceptos.Add(oConcepto);
            //oComprobante.Conceptos = 2stConceptos.ToArray();

            //Linea de Impuestos
            //List<ComprobanteConceptoImpuestosTraslado> lstTraslado7 = new List<ComprobanteConceptoImpuestosTraslado>(); //Cambio
            //oTraslado = new ComprobanteConceptoImpuestosTraslado();
            //oTraslado.Importe = 74.62m;
            //oTraslado.TasaOCuota = 0.160000m;
            //oTraslado.Base = 466.40m;
            //oTraslado.TipoFactor = "Tasa";
            //oTraslado.Impuesto = "002";


            //lstTraslado7.Add(oTraslado); //Cambio
            //lstConceptos.Add(oConcepto);

            //oConcepto.Impuestos = new ComprobanteConceptoImpuestos();

            //oComprobante.Conceptos = lstConceptos.ToArray();
            //oConcepto.Impuestos.Traslados = lstTraslado7.ToArray();//Cambio

            //Fin Concepto 7

            List<ComprobanteImpuestosTraslado> lstCImpuestoT = new List<ComprobanteImpuestosTraslado>();
            // List<ComprobanteImpuestosRetencion> lstCImpuestoR = new List<ComprobanteImpuestosRetencion>();
            ComprobanteImpuestos oCImpuesto = new ComprobanteImpuestos();
            ComprobanteImpuestosTraslado oCImpuestoT = new ComprobanteImpuestosTraslado();
            // ComprobanteImpuestosRetencion oCImpuestoR = new ComprobanteImpuestosRetencion();
            oCImpuesto.TotalImpuestosTrasladados = 551.44m;
            //oCImpuesto.TotalImpuestosRetenidos = 0.00m;

            oCImpuestoT.Importe = 551.44m;
            oCImpuestoT.Impuesto = "002";
            oCImpuestoT.TipoFactor = "Tasa";
            oCImpuestoT.TasaOCuota = 0.160000m;


            //oCImpuestoR.Importe = 4m;
            //oCImpuestoR.Impuesto = "002";

            lstCImpuestoT.Add(oCImpuestoT);
            //lstCImpuestoR.Add(oCImpuestoR);
            oCImpuesto.Traslados = lstCImpuestoT.ToArray();
            //oCImpuesto.Retenciones = lstCImpuestoR.ToArray();
            oComprobante.Impuestos = oCImpuesto;
            //Generar XML 


            // Prueba



            //Prueba


            CreateXML(oComprobante);

            string cadenaOriginal = "";
            string pathxsl = path + @"XSDToXML\cadenaoriginal_3_3.xslt";
            System.Xml.Xsl.XslCompiledTransform transformador = new System.Xml.Xsl.XslCompiledTransform(true);
            transformador.Load(pathxsl);

            using (StringWriter sw = new StringWriter())
            using (XmlWriter xwo = XmlWriter.Create(sw, transformador.OutputSettings))
            {

                transformador.Transform(pathXML, xwo);
                cadenaOriginal = sw.ToString();
            }

            SelloDigital oSelloDigital = new SelloDigital();
            oComprobante.Certificado = oSelloDigital.Certificado(pathCer);
            oComprobante.Sello = oSelloDigital.Sellar(cadenaOriginal, pathKey, clavePrivada);

            CreateXML(oComprobante);
            

            //TIMBRE DEL XML
            ServiceReferenceFC.RespuestaCFDi respuestaCFDI = new ServiceReferenceFC.RespuestaCFDi();

            byte[] bXML = System.IO.File.ReadAllBytes(pathXML);

            ServiceReferenceFC.TimbradoClient oTimbrado = new ServiceReferenceFC.TimbradoClient();

            respuestaCFDI = oTimbrado.TimbrarTest("TEST010101ST1", "aaaaaa", bXML);

            if (respuestaCFDI.Documento == null)
            {
                Console.WriteLine(respuestaCFDI.Mensaje);
            }
            else
            {

                System.IO.File.WriteAllBytes(pathXML, respuestaCFDI.Documento);
            }


        }

        private static void CPago(DataRow dRow)
        {
            string pathCer = path + @"TRCM\TRCM.cer";
            string pathKey = path + @"TRCM\TRCM.key";
            string clavePrivada = "sellorcm970923";
            //string pathCer = path + @"INOVA\INOVA.cer";
            //string pathKey = path + @"INOVA\INOVA.key";
            //string clavePrivada = "Nane1810";

            //Obtenemos el numero
            string numeroCertificado, aa, b, c;
            SelloDigital.leerCER(pathCer, out aa, out b, out c, out numeroCertificado);



            string column = "0";


            Console.WriteLine("inicia : " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            Console.WriteLine("Valor : " + column);


            Comprobante oComprobante = new Comprobante();
            oComprobante.Version = "3.3";
            //oComprobante.Serie = "H";
            oComprobante.Folio = dRow["Folio"].ToString();// "3000001"; //Asi para uno solo de la consulta Consulta debe tomarlo de el campo Folio
            //oComprobante.Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            oComprobante.Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"); //consulta
            // oComprobante.Sello = "faltante"; 
            //oComprobante.FormaPago = "01";
            oComprobante.NoCertificado = numeroCertificado;
            oComprobante.SubTotal = 0m;
            //oComprobante.Descuento = 0;
            oComprobante.Moneda = "XXX";
            oComprobante.Total = 0m;
            oComprobante.TipoDeComprobante = "P";
            // oComprobante.MetodoPago = "PPD";
            oComprobante.LugarExpedicion = dRow["LExpedicion"].ToString();//"66266"; //consulta

            dRow["Folio"].ToString();
            //string pathXML = path + @"PrimarioISI121121I31365.xml"; //Config
            string pathXML = pathl + dRow["NArchivo"].ToString() + ".xml";//Config


            ComprobanteEmisor oEmisor = new ComprobanteEmisor();

            oEmisor.Rfc = dRow["EmisorRFC"].ToString();//"RCM970923SD1"; //consulta
            oEmisor.Nombre = dRow["EmisorNombre"].ToString();//"The Right Connection México S.A. de C.V."; //consulta
            oEmisor.RegimenFiscal = "601"; //consulta

            ComprobanteReceptor oReceptor = new ComprobanteReceptor();
            oReceptor.Nombre = dRow["ReceptorNombre"].ToString();//"MAYA MOTRIZ, S.A. DE C.V.";//consulta
            oReceptor.Rfc = dRow["ReceptorRFC"].ToString();//"ACA830901ML5";//consulta
            oReceptor.UsoCFDI = "P01";

            //asigno emisor y receptor
            oComprobante.Emisor = oEmisor;
            oComprobante.Receptor = oReceptor;


            List<ComprobanteConcepto> lstConceptos = new List<ComprobanteConcepto>();
            ComprobanteConcepto oConcepto = new ComprobanteConcepto();
            oConcepto.Importe = 0;
            oConcepto.ClaveProdServ = "84111506";
            oConcepto.Cantidad = 1;
            oConcepto.ClaveUnidad = "ACT";
            oConcepto.Descripcion = "Pago";
            oConcepto.ValorUnitario = 0;
            //oConcepto.Descuento = 0;


            lstConceptos.Add(oConcepto);

            oComprobante.Conceptos = lstConceptos.ToArray();

            //complemento Pago
            Pagos oPagos = new Pagos();
            List<PagosPago> lstPagos = new List<PagosPago>();
            PagosPago oPago = new PagosPago();
            oPago.MonedaP = dRow["MonedaP"].ToString();//"USD";//consulta
            oPago.TipoCambioP = Math.Round(Convert.ToDecimal(dRow["TipoCambioPago"]), 4);
            if (Math.Round(Convert.ToDecimal(dRow["TipoCambioPago"]), 4) == 0) {
                oPago.TipoCambioPSpecified = false;
            }
            else
            {
                oPago.TipoCambioPSpecified = true;
            }
            
            switch (dRow["FormaPagoP"].ToString())
            { case "01":
                    oPago.FormaDePagoP = c_FormaPago.Item01;
                    break;
                case "02":
                    oPago.FormaDePagoP = c_FormaPago.Item02;
                    break;
                case "03":
                    oPago.FormaDePagoP = c_FormaPago.Item03;
                    break;
            }
            //oPago.FormaDePagoP = Convert.ToInt32(1);// dRow["FormaPagoP"].ToString();//c_FormaPago.Item01;//consulta
            oPago.Monto = Math.Round((Convert.ToDecimal(dRow["Monto"])), 2);//1.16m; //consulta
            //oPago.FechaPago = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //oPago.FechaPago = "2018-03-01T01:13:32";
            oPago.FechaPago = dRow["FechaPago"].ToString();//consulta
            //DoctoRelacionado de Pago

            //decimal d4 = decimal.Parse(sqlra[12].ToString());
            //oPago.TipoCambioP = d4;
            //oPago.TipoCambioPSpecified = true;

            //Listar
            //Esto para que es aqui? cada folio puede tener n lineas con el mismo numero de folio , lo que hace es que agrega los documentos que pertenecen a ese folio , ejemplo , el folo es una pago y las lineas las facturas que de pagaron en ese folio de pago
            String sql = String.Format("SELECT DISTINCT [ImpSaldoInsoluto],[ImpPagado],[ImpSaldoAnt],[MonedaDR],[NParcialidad],[Ffactura],[IdDocumento],[MetodoDePagoDR],[NArchivo], MonedaP, [TipoCambioPago] FROM [CERTIFICA].[dbo].[xPago] WHERE Folio = '{0}'", dRow["Folio"]);
            //String sqlU = String.Format("UPDATE [CERTIFICA].[dbo].[xPago] SET [Status] = 1 WHERE [Folio] = '{0}'", dRow["Folio"]);
            sData = new SqlDataAdapter(sql, Cnx);
            dDataDetalle.Clear();
            if (sData.Fill(dDataDetalle) > 0)
            {
                List<PagosPagoDoctoRelacionado> lstDoctos = new List<PagosPagoDoctoRelacionado>();
                foreach (DataRow item in dDataDetalle.Rows)
                {
                    PagosPagoDoctoRelacionado oDoctoRelacionado = new PagosPagoDoctoRelacionado();


                    oDoctoRelacionado = new PagosPagoDoctoRelacionado();
                    oDoctoRelacionado.IdDocumento = item["IdDocumento"].ToString(); //"D9A14C9E-9D93-46AE-89EF-0F6EB6BC04A4";//consulta
                    oDoctoRelacionado.MonedaDR = item["MonedaP"].ToString(); // "MXN";//consulta.
                    //oDoctoRelacionado.TipoCambioDR = Math.Round(Convert.ToDecimal(item["TipoCambioPago"]), 4);
                    //oDoctoRelacionado.TipoCambioDRSpecified = true;

                    oDoctoRelacionado.ImpPagado = Math.Round((Convert.ToDecimal(item["ImpPagado"])), 2); //1.16m;//consulta
                    oDoctoRelacionado.MetodoDePagoDR = c_MetodoPago.PPD;//consulta Debes cambiar esto
                    oDoctoRelacionado.NumParcialidad = item["NParcialidad"].ToString();//"1";//consulta


                    oDoctoRelacionado.ImpSaldoAnt = Math.Round((Convert.ToDecimal(item["ImpSaldoAnt"])), 2);//1.16m;//consulta

                    oDoctoRelacionado.ImpSaldoInsoluto = Convert.ToDecimal(item["ImpSaldoInsoluto"]);//0.00m;//consulta
                    oDoctoRelacionado.Folio = item["Ffactura"].ToString();//"3000004";//consulta

                    lstDoctos.Add(oDoctoRelacionado);
                }
                oPago.DoctoRelacionado = lstDoctos.ToArray();
                //UpCertifica(dRow["Folio"].ToString());
            }

            
            lstPagos.Add(oPago);
            oPagos.Pago = lstPagos.ToArray();

            oComprobante.Complemento = new ComprobanteComplemento[1];
            oComprobante.Complemento[0] = new ComprobanteComplemento();

            XmlDocument docPago = new XmlDocument();
            XmlSerializerNamespaces xmlNameSpacePago = new XmlSerializerNamespaces();
            //xmlNameSpacePago.Add("p1", "http://www.w3.org/2001/XMLSchema-instance");
            xmlNameSpacePago.Add("pago10", "http://www.sat.gob.mx/Pagos");
            using (XmlWriter writer = docPago.CreateNavigator().AppendChild())
            {
                new XmlSerializer(oPagos.GetType()).Serialize(writer, oPagos, xmlNameSpacePago);
            }
            oComprobante.Complemento[0].Any = new XmlElement[1];
            oComprobante.Complemento[0].Any[0] = docPago.DocumentElement;



            //Creamos el xml
            CreateXML(oComprobante, pathXML);

            string cadenaOriginal = "";
            string pathxsl = path + @"XSDToXML\cadenaoriginal_3_3.xslt";
            //string pathxsl = path + @"c:\temp\cadenaoriginal_3_3.xslt";
            System.Xml.Xsl.XslCompiledTransform transformador = new System.Xml.Xsl.XslCompiledTransform(true);
            transformador.Load(pathxsl);

            using (StringWriter sw = new StringWriter())
            using (XmlWriter xwo = XmlWriter.Create(sw, transformador.OutputSettings))
            {

                transformador.Transform(pathXML, xwo);
                cadenaOriginal = sw.ToString();
            }


            SelloDigital oSelloDigital = new SelloDigital();
            oComprobante.Certificado = oSelloDigital.Certificado(pathCer);
            oComprobante.Sello = oSelloDigital.Sellar(cadenaOriginal, pathKey, clavePrivada);

            CreateXML(oComprobante, pathXML);
            UpCertifica(dRow["Folio"].ToString());
            MueveArchivo(pathXML.ToString(), dRow["NArchivo"].ToString());

            Console.WriteLine("termina : " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
        }

        private static void CreateXML(Comprobante oComprobante, string ruta)
        {
            //SERIALIZAMOS.-------------------------------------------------

            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
            //xmlNameSpace.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
            xmlNameSpace.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlNameSpace.Add("pago10", "http://www.sat.gob.mx/Pagos");



            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));

            string sXml = "";

            using (var sww = new Utils.StringWriterWithEncoding(Encoding.UTF8))
            {

                using (XmlWriter writter = XmlWriter.Create(sww))
                {

                    oXmlSerializar.Serialize(writter, oComprobante, xmlNameSpace);
                    sXml = sww.ToString();
                }

            }

            //guardamos el string en un archivo
            System.IO.File.WriteAllText(ruta, sXml);
        }

        private static void CreateXML(Comprobante oComprobante)
        {
            //SERIALIZAMOS.-------------------------------------------------

            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
            xmlNameSpace.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
            xmlNameSpace.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlNameSpace.Add("pago10", "http://www.sat.gob.mx/Pagos");

            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));

            string sXml = "";

            using (var sww = new Utils.StringWriterWithEncoding(Encoding.UTF8))
            {

                using (XmlWriter writter = XmlWriter.Create(sww))
                {

                    oXmlSerializar.Serialize(writter, oComprobante, xmlNameSpace);
                    sXml = sww.ToString();
                }

            }

            //guardamos el string en un archivo
            System.IO.File.WriteAllText(pathXML, sXml);
           



        }
        private static void UpCertifica(string pFolioD)
        {
            string constrDN = "Data Source = 192.168.2.72; Initial Catalog = CERTIFICA; Persist Security Info = True; User ID = sa; password = *SaP5950*; Pooling = True; ";
            using (SqlConnection conDN = new SqlConnection(constrDN))
            {
                var pSqlDN = "";
                pSqlDN += "UPDATE [CERTIFICA].[dbo].[xPago] SET[Status] = 1 where Folio = " + pFolioD;
                using (SqlCommand cmdD = new SqlCommand(pSqlDN))
                {
                    cmdD.Connection = conDN;
                    conDN.Open();
                    cmdD.ExecuteNonQuery();
                    cmdD.Dispose();
                }
                conDN.Close();
            }

        }
        private static void MueveArchivo(string Rutax,string NombreFinal)
        {
            string pFileDN = "";
            pFileDN = Rutax;
            File.Copy(pFileDN, @"\\192.168.2.21\b1_shr\FAE\Entrada\" + NombreFinal + ".xml");

        }
    }
}
