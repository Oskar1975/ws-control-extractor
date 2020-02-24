using DataExtractor.Common;
using EntiExtractor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace ServicioExtractor
{
    public partial class ServExtractor : ServiceBase
    {

        #region "Variables"

        private cSecurity cSecure;
        private object poTablaLock; 
        private Timer Tmrprincipal;
        private bool PuedeProcesarTimer;
        private bool DetenerProceso;
        private DataTable ProcesosCorriendo = null;
        private string psMsgError = string.Empty;

        #endregion

        private object TablaLock
        {
            set
            {
                poTablaLock = value;
            }
            get
            {
                if (poTablaLock == null)
                    poTablaLock = new object();
                return poTablaLock;
            }
        }


        public ServExtractor()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            ProcesosCorriendo = new DataTable();
            ProcesosCorriendo.Columns.Add("IdConfExtraccion");
            ProcesosCorriendo.Columns.Add("TotalFallas").DefaultValue = 0;

            Tmrprincipal = new Timer(60 * 10000); //Tiemp en Milisegundos
            Tmrprincipal.AutoReset = true;
            Tmrprincipal.Enabled = true;
            PuedeProcesarTimer = true;
            DetenerProceso = true;

        }

        protected override void OnStop()
        {
            PuedeProcesarTimer = false;
            Tmrprincipal.Stop();
            Tmrprincipal.Dispose();
        }

        private void ProcesaTimer(object sender, ElapsedEventArgs e)
        {

            DataTable LoDtTareas = null;
            string lsMensaje = String.Empty;

            //Verificamos si puede realizar el evento
            if (!PuedeProcesarTimer || ProcesosCorriendo.Rows.Count >= 5)
                return;

            try
            {
                //Detenemos el servicio de busqueda momentaneamente
                Tmrprincipal.Stop();
                PuedeProcesarTimer = false;
                //Buscamos las tareas pendientes a realizar

                if (!validarConexiones(ref lsMensaje))
                    return;

                LoDtTareas = getTareas(ref lsMensaje);

                //Recorremos las tareas y determinamos cual se debe ejecutar
                foreach(DataRow loRow in LoDtTareas.Rows)
                { 
                    if(estaCorriendoExtrac(Convert.ToInt32(loRow["IdConfExtraccion"])))
                        continue;
                  
                    if (DetenerProceso)
                        break;

                    //Asignamos la tarea
                    System.Threading.Thread loTarea = new System.Threading.Thread(EjecutaProceso);
                    creaExtrac(Convert.ToInt32(loRow["IdConfExtraccion"]));

                    loTarea.IsBackground = true;
                    loTarea.CurrentCulture = new System.Globalization.CultureInfo("es-MX");
                    loTarea.CurrentUICulture = new System.Globalization.CultureInfo("es-MX");

                    loTarea.Name = loRow["IdConfExtraccion"].ToString();
                    loTarea.Start(loRow["IdConfExtraccion"]);

                    //Si ya revasamos el limite de asignaciones nos salimos
                    if (ProcesosCorriendo.Rows.Count >= 50)
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void EjecutaProceso(object toIdConfExtraccion)
        {            
            string lsMensaje = string.Empty;            
            int liHilosVivos = 0;
            System.Threading.Thread[] Colhilos = new System.Threading.Thread[50];
            bEntiConfExtra loConfExtra =  new bEntiConfExtra(); ;
            bEntiServExtractor loServExtrac;
            bEntiDatosExtract loDatosExtract;
            bEntiDatosEntrada loDatoEntrada;
            bEntiConfParEmpresa loParamEmpresa;                     
            bEntiConfParametro loConfParametro;

            try
            {                
                loDatosExtract = new bEntiDatosExtract();
                loDatoEntrada = new bEntiDatosEntrada();
                loConfParametro = new bEntiConfParametro();
                loParamEmpresa = new bEntiConfParEmpresa();

                //Buscar la información de la configuración de extracción
                loConfExtra.RowNumber = Convert.ToInt32(toIdConfExtraccion);

                //Buscar la información de datos de extracción a partir de la configuración de extracción
                loDatosExtract.IdConfExtraccion = loConfExtra.RowNumber;
                List< bEntiDatosExtract> laDatosExtract = new List<bEntiDatosExtract>();

                //Buscar la lista de configuración de empresas para la extracción a partir de la configuración de la extracción
                loParamEmpresa.IdConfExtraccion = loConfExtra.RowNumber;
                List<bEntiConfParEmpresa> laParamEmpresa = new List<bEntiConfParEmpresa>();

                //Guardamos la cabecera del log
                loServExtrac = new bEntiServExtractor();
                loServExtrac.Estatus = "ACTIVO";
                loServExtrac.FechaEvento = DateTime.Now;
                loServExtrac.HoraEvento = DateTime.Now;
                loServExtrac.EstusEvento = bEntiServExtractor.lnEstusEvento.Procesado;
                loServExtrac.FechaConfigurador = DateTime.Now; //loConfExtra.ProximaFecha.Date;
                loServExtrac.HoraConfigurador = DateTime.Now; //loConfExtra.HoraExtraccion;
                loServExtrac.IdConfExtraccion = 0; //loConfExtra.RowNumber;
                loServExtrac.RowNumber = 0;
                loServExtrac.FolioEvento = 0;
                loServExtrac.ClaveLogin = ""; //loConfExtra.ClaveLogin;
                
                /*
                if (!guardarloServExtrac())
                {
                    string lsErr = "No se pudo guardar el evento de extraccion: "
                    + "\n IdConfExtraccion=" + loConfExtra.RowNumber.ToString()
                    + "\nFecha Configurador=" + String.Format(loConfExtra.ProximaFecha,
                        "dd/MM/yyyy")
                    + "\nHora Extracción=" + String.Format(loConfExtra.HoraExtraccion, 
                        "HH:mm ss") + "\nMotivo:" + "Error";

                    validarError(0, lsErr,"AMBIENTE", "Error");
                    EnviaCorreo("No se pudo guardar el evento de extraccion", _
                    + "\nNo se pudo guardar el evento de extraccion: "
                    + "\nIdConfExtraccion=" + loConfExtra.RowNumber.ToString()
                    + "Fecha Configurador=" + String.Format(loConfExtra.ProximaFecha,
                    "dd/MM/yyyy") 
                    + "\nHora Extracción=" & String.Format(loConfExtra.HoraExtraccion, 
                    "HH:mm ss") + "\nMotivo:" + "Error", true);
                    return;
                }
                else
                {
                    //Actualizamos el folio del evento
                    loServExtrac.FolioEvento = loServExtrac.RowNumber;

                    actualizarloServExtrac();
                }
                */                

                foreach (bEntiDatosExtract loTmpDatosExtract in laDatosExtract)
                {
                    if (loTmpDatosExtract.Estatus.ToUpper() != "ACTIVO")
                        return;

                    if (DetenerProceso)
                        return;

                    foreach (bEntiConfParEmpresa loTmpParamEmpresa in laParamEmpresa)
                    {
                        //Por cada parametro de empresa, ejecutamos un subproceso
                        if (loTmpParamEmpresa.Estatus.ToUpper() != "ACTIVO")
                            return;

                        if (DetenerProceso)
                            return;

                        //Buscamos su Dato de Entrada
                        loDatoEntrada.RowNumber = loTmpDatosExtract.IdDatosEntrada;

                        do
                        {
                            //Asignamos la solicitud al hilo disponible
                            for (int hilo = 0; hilo < 50 - 1; hilo++)
                            {
                                if (DetenerProceso)
                                    return;

                                if (Colhilos[hilo] != null || Colhilos[hilo].IsAlive == false)
                                {
                                    Colhilos[hilo] = new System.Threading.Thread(EjecutaSubProceso);
                                    Colhilos[hilo].IsBackground = true;
                                    Colhilos[hilo].CurrentCulture = new System.Globalization.CultureInfo("es-MX");
                                    Colhilos[hilo].CurrentUICulture = new System.Globalization.CultureInfo("es-MX");

                                    bEntiParamSubProceso loParamSubProceso = new bEntiParamSubProceso();
                                    loParamSubProceso.oServExtracDet = new bEntiServExtractorDet();

                                    loParamSubProceso.IdConfExtraccion = loConfExtra.RowNumber;
                                    loParamSubProceso.Ambiente = ""; //DataBase.Ambiente;
                                    loParamSubProceso.ClaveEmpresa = loTmpParamEmpresa.ClaveEmpresa;
                                    loParamSubProceso.CveDatoEntrada = loDatoEntrada.Clave;
                                    /*
                                    if(loConfParametro.TipoEjercicio =  ConfParametro.lnTipoEjercicio.Fijo)
                                        loParamSubProceso.Ejercicio = loConfParametro.EjercicioAnio;
                                    else
                                        loParamSubProceso.Ejercicio = Now.Year - loConfParametro.EjercicioMenos;                                   
                                    */

                                    /*
                                    loParamSubProceso.Periodo = GetNumPeriodo(loConfParametro.PeriodoMenos,
                                        loConfParametro.PeriodoNum,
                                        loConfParametro.TipoPeriodo);
                                    */

                                    loParamSubProceso.Usuario = loConfExtra.ClaveLogin;

                                    loParamSubProceso.oServExtracDet.ClaveLogin = loConfExtra.ClaveLogin;
                                    loParamSubProceso.oServExtracDet.ClaveEmpresa = loTmpParamEmpresa.ClaveEmpresa;
                                    loParamSubProceso.oServExtracDet.Estatus = "ACTIVO";
                                    loParamSubProceso.oServExtracDet.EstusEvento = bEntiServExtractorDet.lnEstusEvento.EnEjecucion; 
                                    loParamSubProceso.oServExtracDet.FechaAlta = DateTime.Now; 
                                    loParamSubProceso.oServExtracDet.FechaEvento = DateTime.Now; 
                                    loParamSubProceso.oServExtracDet.HoraAlta = DateTime.Now; 
                                    loParamSubProceso.oServExtracDet.GeneroAlarma = false;
                                    loParamSubProceso.oServExtracDet.HoraEvento = DateTime.Now;
                                    loParamSubProceso.oServExtracDet.IdAlarma = 0;
                                    loParamSubProceso.oServExtracDet.IdDatosEntrada = loDatoEntrada.RowNumber;
                                    loParamSubProceso.oServExtracDet.IdEmpresa = loTmpParamEmpresa.IdEmpresa;
                                    loParamSubProceso.oServExtracDet.IdServExt = loServExtrac.RowNumber;
                                    loParamSubProceso.oServExtracDet.Mensaje = string.Empty;
                                    loParamSubProceso.oServExtracDet.NumEjercicio = loParamSubProceso.Ejercicio;
                                    loParamSubProceso.oServExtracDet.NumPeriodo = loParamSubProceso.Periodo;

                                    Colhilos[hilo].Start(loParamSubProceso);
                                    break;
                                }
                            }

                        } while (true);
                    }
                }


                //Esperamos a que todos los hilos terminen
                do
                {
                    liHilosVivos = 0;
                    for (int liHilo = 0; liHilo < 50; liHilo++)
                    {
                        if (Colhilos[liHilo] != null &&
                            Colhilos[liHilo].IsAlive)
                            liHilosVivos += 1;                        
                    }
                    if (liHilosVivos == 0)
                        break;

                    //Esperamos un momento
                    System.Threading.Thread.Sleep(2000);
                } while (true);

            }
            catch (OracleException ex)
            {
                validarError(ex.Code, ex.Message, "", ex.Message);
            }
            catch (Exception ex)
            {
                int liIdConfExtraccion = 0;
                if (loConfExtra != null)
                    liIdConfExtraccion = loConfExtra.RowNumber;

                validarError(0, "Error al ejecutar proceso. " + ex.Message, "", ex.Message);

                //EnviaCorreo("Proceso extracción","Se presentó una falla inesperada en el proceso de extraccion.\n"
                // + "IdConfExtraccion=" + liIdConfExtraccion.ToString + "\n" +
                // "Motivo:" + lsMensaje, true);
            }
            finally
            {
                finalizaExtrac(Convert.ToInt32(toIdConfExtraccion));
            }    
        }

        private void EjecutaSubProceso(object toParamSubProceso)
        {
            string lsMsgError = string.Empty;
            bEntiParamSubProceso loParamEntrada = (bEntiParamSubProceso)toParamSubProceso;

            try
            {
                if (DetenerProceso)
                    return;

                loParamEntrada.oServExtracDet.FechaEvento = DateTime.Now;
                loParamEntrada.oServExtracDet.HoraEvento = DateTime.Now;

                //Guardar los datos del loParamEntrada.oServExtracDet en la Base de Datos


                //verificar los parametros y la empresa si es cloud o no                
                // loParamEntrada.ClaveEmpresa 
                if(true)
                {
                    //si es cloud se invoca los wsdl


                    //si no es cloud se invocan los packages del EBS
                    if (ejecutarEBS(ref lsMsgError))
                    {
                        loParamEntrada.oServExtracDet.EstusEvento = bEntiServExtractorDet.lnEstusEvento.Procesado;
                        loParamEntrada.oServExtracDet.Mensaje = string.Empty;
                    }
                    else
                    {
                        string lsErr = "Error al ejecutar subproceso. " + lsMsgError;

                        if (lsMsgError.Contains("1017"))
                            validarError(1017, lsErr, loParamEntrada.Ambiente, lsMsgError);
                        else if (lsMsgError.Contains("2800"))
                            validarError(2800, lsErr, loParamEntrada.Ambiente, lsMsgError);
                        else if (lsMsgError.Contains("12541"))
                            validarError(12541, lsErr, loParamEntrada.Ambiente, lsMsgError);
                        else
                            validarError(0, lsErr, loParamEntrada.Ambiente, lsMsgError);

                        //Marcamos error
                        loParamEntrada.oServExtracDet.EstusEvento = bEntiServExtractorDet.lnEstusEvento.ProcesadoError;
                        loParamEntrada.oServExtracDet.Mensaje = lsMsgError;
                    }
                }                

                loParamEntrada.oServExtracDet.FechaFinEvento = DateTime.Now;
                loParamEntrada.oServExtracDet.HoraFinEvento = DateTime.Now;

                //Acumulamos los Errores
                if (loParamEntrada.oServExtracDet.EstusEvento !=
                    bEntiServExtractorDet.lnEstusEvento.Procesado)
                    acumulaErroresExtrac(loParamEntrada.IdConfExtraccion);

                //if (loParamEntrada.oServExtracDet.Update)
                {
                    validarError(0, "error",
                        loParamEntrada.Ambiente, "error");
                    //EnviaCorreo("Proceso extracción",
                    //"Se presentó una falla inesperada al intentar actualizar "
                    //+ "el estatus de la extracción.\n"
                    //+ "IdConfExtraccion=" + loParamEntrada.IdConfExtraccion +
                    //"EstatusEvento=" + loParamEntrada.oServExtracDet.EstusEvento.ToString()
                    //+ "\nMotivo:"
                    //+ loParamEntrada.oServExtracDet.Mapper.LastError +
                    //+"\nAmbiente:" +
                    //loParamEntrada.Ambiente, true);
                }


                //Por último enviamos la alarma
                //Falta definición de alarma
                if (loParamEntrada.oServExtracDet.EstusEvento !=
                    bEntiServExtractorDet.lnEstusEvento.Procesado)
                {
                    string lsErr = "Error al procesar: " + loParamEntrada.oServExtracDet.Mensaje;
                    validarError(0, lsErr, loParamEntrada.Ambiente, loParamEntrada.oServExtracDet.Mensaje);
                    //EnviaCorreo("Alarma: Proceso extracción",
                    //"Se presentó una falla en el proceso de extraccion.\n"
                    //+ "Folio Evento=" + loParamEntrada.IdConfExtraccion.ToString()
                    //+ "\nEjercicio=" + loParamEntrada.oServExtracDet.NumEjercicio.ToString()
                    //+ "\nPeriodo=" + loParamEntrada.oServExtracDet.NumPeriodo.ToString()
                    //+ "\nClave Emprresa=" + loParamEntrada.oServExtracDet.ClaveEmpresa
                    //+ "\nMotivo:" + loParamEntrada.oServExtracDet.Mensaje
                    //+ "\nAmbiente:" + loParamEntrada.Ambiente, false);
                }


            }
            catch (OracleException ex)
            {
                validarError(ex.Code, ex.Message, "", ex.Message);
            }
            catch (Exception ex)
            {
                validarError(0, "Error al ejecutar subproceso. " + ex.Message, "", ex.Message);

                //EnviaCorreo("Proceso extracción","Se presentó una falla inesperada en la extraccion de datos.\n"
                // + "Motivo: " + ex.Message + "\n" +
                // "Ambiente: ", true);
            }
          
        }

        private void EnviaCorreo(string v1, object p, bool v2)
        {
            throw new NotImplementedException();
        }

        private bool ejecutarEBS(ref string tsMsjError)
        {
            return true;
        }

        private void acumulaErroresExtrac(int tiIdConfExtraccion)
        {
            lock (TablaLock)
            {
                DataRow[] loFilas;

                loFilas = ProcesosCorriendo.Select("IdConfExtraccion=" + tiIdConfExtraccion.ToString());
                if (loFilas.Length > 0)
                    loFilas[0]["TotalFallas"] = System.Convert.ToInt32(loFilas[0]["TotalFallas"]) + 1;
            }
        }

        private void creaExtrac(int tiIdConfExtraccion)
        {
            lock (TablaLock)
            {
                System.Threading.Thread loTarea = new System.Threading.Thread(EjecutaProceso);
                ProcesosCorriendo.Rows.Add(new object[] { tiIdConfExtraccion });
            }
        }

        private void finalizaExtrac(int tiIdConfExtraccion)
        {
            lock (TablaLock)
            {
                DataRow[] loFilas = null;
                loFilas = ProcesosCorriendo.Select("IdConfExtraccion=" + tiIdConfExtraccion.ToString());
                if (loFilas.Length > 0)
                    ProcesosCorriendo.Rows.Remove(loFilas[0]);
            }
        }

        private bool estaCorriendoExtrac(int tiIdConfExtraccion)
        {
            lock (TablaLock)

            return ProcesosCorriendo.Select("IdConfExtraccion=" + tiIdConfExtraccion.ToString()).Length > 0;
        }

        private DataTable getEstructuraTareas()
        {
            DataTable loDtTareas = new DataTable();

            loDtTareas.Columns.Add("IdConfExtraccion");
            loDtTareas.Columns.Add("ProximaFecha");
            loDtTareas.Columns.Add("FechaFinal");
            loDtTareas.Columns.Add("EsInfinito");
            loDtTareas.Columns.Add("CantidadEventos");
            
            return loDtTareas;
        }

        private DataTable getTareas(ref string tsErrMessage)
        {
            DataTable loDtTareas = getEstructuraTareas();
            OracleDataAdapter loAdaptador;
            OracleConnection loConexion = null;
            DataTable loDtConsulta;

            try
            {
                if (!GetConexion(ref loConexion, ref tsErrMessage))
                    return loDtTareas;

                loConexion.Open();
                loAdaptador = new OracleDataAdapter("XXFM.XXFM_INT_CONF_EXT_READ_U02", loConexion);

                loAdaptador.SelectCommand.CommandType = CommandType.StoredProcedure;
                loAdaptador.SelectCommand.Connection = loConexion;
                loAdaptador.SelectCommand.Parameters.Add(new OracleParameter("pcurcursor", OracleType.Cursor));
                loAdaptador.SelectCommand.Parameters["pcurcursor"].Direction = ParameterDirection.Output;

                loDtConsulta = new DataTable("DtConsulta");
                loAdaptador.Fill(loDtConsulta);
                foreach (DataRow loRow in loDtConsulta.Rows)
                {
                    DataRow loTmpRow = loDtTareas.NewRow();

                    loTmpRow["IdConfExtraccion"] = loRow["ID_CONFE"];
                    loTmpRow["ProximaFecha"] = loRow["FEC_CONFE"];
                    loTmpRow["FechaFinal"] = loRow["FEC_FIN"];
                    //loTmpRow["EsInfinito"] = loRow["COD_FIN"].ToString() = "T";
                    loTmpRow["CantidadEventos"] = loRow["NUM_RECU"];

                    loDtTareas.Rows.Add(loTmpRow);
                }

                //Una vez recuperada las tareas se cambia su estatus a INACTIVO inmediatamente
                //Esto con la finalidad de evitar que el sistema vuelva
                //a tomar extracciones que ya están siendo o fueron procesadas

                foreach (DataRow loRow in loDtConsulta.Rows)
                {
                    //TODO
                }

            }
            catch (OracleException ex)
            {
                validarError(ex.Code, ex.Message, "", ex.Message);
            }
            catch (Exception ex)
            {
                validarError(0, ex.Message, "", ex.Message);
            }
            finally
            {
                if (loConexion != null)
                    loConexion.Close();                
            }                     

            //Retornamos el resultado
            return loDtTareas;
        }

        private Boolean GetConexion(ref OracleConnection toConexion, ref string tsErrmsj)
        {
            string lsCadenaCon = String.Empty;
            bool lbRetval = true;
            string lsAmbiente = "";
            cSecure = new cSecurity();

            try
            {
                lsCadenaCon = cSecure.getConnection("","");
                toConexion = new OracleConnection(lsCadenaCon);
            }
            catch (InvalidOperationException ex)
            {
                tsErrmsj += ex.Message;
                lbRetval = false;
                validarError(0, ex.Message, lsAmbiente, ex.Message);
            }
            catch (OracleException ex)
            {
                tsErrmsj += ex.Message;
                lbRetval = false;
                validarError(ex.Code, ex.Message, lsAmbiente, ex.Message);
            }
            catch (Exception ex)
            {
                tsErrmsj += ex.Message;
                lbRetval = false;
                validarError(0, ex.Message, lsAmbiente, ex.Message);
            }

            return lbRetval;

        }

        private bool validarConexiones(ref string tsErrmsj)
        {
            string lsCadenaCon = String.Empty;
            bool lbRetval = true;
            string lsAmbiente = String.Empty;
            cSecure = new cSecurity();

            try
            {
                //Validamos las conexiones a FiscalX
                lsAmbiente = "ORAPREQA";
                lsCadenaCon = cSecure.getConnection("",lsAmbiente);
                OracleConnection loConexion = new OracleConnection(lsCadenaCon);

                loConexion.Open();
                loConexion.Close();

                // Validamos las conexiones a TV
                lsAmbiente = "TVQA";
                lsCadenaCon = cSecure.getConnection("", lsAmbiente);
                loConexion = new OracleConnection(lsCadenaCon);

                loConexion.Open();
                loConexion.Close();

                // Validamos las conexiones a TVMEX
                lsAmbiente = "TVMEXQA";
                lsCadenaCon = cSecure.getConnection("", lsAmbiente);
                loConexion = new OracleConnection(lsCadenaCon);

                loConexion.Open();
                loConexion.Close();


            }
            catch (System.InvalidOperationException ex)
            {
                tsErrmsj += ex.Message;
                lbRetval = false;
                validarError(0, tsErrmsj, lsAmbiente, ex.Message);
            }
            catch (OracleException ex)
            {
                tsErrmsj += ex.Message;
                lbRetval = false;
                validarError(ex.Code, tsErrmsj, lsAmbiente, ex.Message);
            }
            catch (System.Exception ex)
            {
                tsErrmsj += ex.Message;
                lbRetval = false;
                validarError(0, tsErrmsj, lsAmbiente, ex.Message);
            }

            return lbRetval;

        }

        private void validarError(int tiNumError, string tsErrorMsg, string tsAmbiente, string tsMsgBan)
        {
            if (tiNumError == 1017)
            {
                //EnviaCorreo("Error en el Servicio de Extraccion",
                //      "El servicio de extracción de " + tsAmbiente
                //       + " se detuvo debido a que ha cambiado la contraseña del usuario XXFM.\n" 
                //       + "Favor de actualizar la contraseña y reanudar el servicio.", true);
                DetenerProceso = true;
                psMsgError = "USUARIO_INVALIDO";
            }
            else if (tiNumError == 28000)
            {
                //EnviaCorreo("Error en el Servicio de Extraccion",
                //       "El servicio de extracción de " + tsAmbiente 
                //       + " se detuvo debido a que el usuario XXFM esta bloqueado.\n"
                //       + "Favor de desbloquear el usuario y reanudar el servicio.", true);
                DetenerProceso = true;
                psMsgError = "USUARIO_BLOQUEADO";
            }
            else if (tiNumError == 12541)
            {
                //EnviaCorreo("Error en el Servicio de Extraccion",
                //       "El servicio de extracción de " + tsAmbiente 
                //       + " se detuvo debido a que la instancia de Base da Datos está Off.", true);
                DetenerProceso = true;
                psMsgError = "INSTANCIA_INCORRECTA";
            }
            else if (tiNumError == 12514)
            {
                //EnviaCorreo("Error en el Servicio de Extraccion",
                //       "El servicio de extracción de " + tsAmbiente 
                //       + " se detuvo debido a que la instancia de Base da Datos está Off.", true);
                DetenerProceso = true;
                psMsgError = "SERVIDOR_INCORRECTO";
            }
            else if (tiNumError == 12170)
            {
                //EnviaCorreo("Error en el Servicio de Extraccion",
                //       "El servicio de extracción de " + tsAmbiente 
                //       + " se detuvo debido a que la instancia de Base da Datos está Off.", true);
                DetenerProceso = true;
                psMsgError = "HOST_INCORRECTO";
            }
            else
            {
                //EnviaCorreo("Falla Inesperada",
                //       "Se presentó un error inesperado."
                //       + "Motivo:" + tsErrorMsg, true);
                psMsgError = "EN_EJECUCION_DEL_SERVICIO";
            }

        }       
    }
}
