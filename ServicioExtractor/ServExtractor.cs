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
                    if(estaCorriendoExtrac(int.Parse(loRow["IdConfExtraccion"].ToString())))
                        continue;
                  
                    if (DetenerProceso)
                        break;

                    //Asignamos la tarea
                    System.Threading.Thread loTarea = new System.Threading.Thread(EjecutaProceso);
                    creaExtrac(int.Parse(loRow["IdConfExtraccion"].ToString()));

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
            object loConfExtra = null;
            System.Threading.Thread[] Colhilos = new System.Threading.Thread[50];

            try
            {
                //Buscar la información de la configuración de extracción


                //Buscar la información de datos de extracción a partir de la configuración de extracción
                DataTable laDatosExtract = new DataTable();

                //Buscar la lista de configuración de empresas para la extracción a partir de la configuración de la extracción
                DataTable laParamEmpresa = new DataTable();

                foreach (DataRow loDatosExtract in laDatosExtract.Rows)
                {
                    if (loDatosExtract["COD_ESTATUS"].ToString().ToUpper() != "ACTIVO")
                        return;

                    if (DetenerProceso)
                        return;

                    foreach(DataRow loParamEmpresa in laParamEmpresa.Rows)
                    {
                        if (loParamEmpresa["COD_ESTATUS"].ToString().ToUpper() != "ACTIVO")
                            return;

                        if (DetenerProceso)
                            return;

                        do
                        {
                            //Asignamos la solicitud al hilo disponible
                            for (int hilo = 0; hilo < 50 -1; hilo++)
                            {
                                if (DetenerProceso)
                                    return;

                                if (Colhilos[hilo] != null || Colhilos[hilo].IsAlive == false)
                                {
                                    Colhilos[hilo] = new System.Threading.Thread(EjecutaSubProceso);
                                    Colhilos[hilo].IsBackground = true;
                                    Colhilos[hilo].CurrentCulture = new System.Globalization.CultureInfo("es-MX");
                                    Colhilos[hilo].CurrentUICulture = new System.Globalization.CultureInfo("es-MX");

                                    ParamSubProceso loParamSubProceso = new ParamSubProceso();
                                    loParamSubProceso.oServExtracDet = new DataTable();

                                    loParamSubProceso.IdConfExtraccion = 1; //loConfExtra.RowNumber
                                    loParamSubProceso.Ambiente = ""; //DataBase.Ambiente;
                                    loParamSubProceso.ClaveEmpresa = ""; //loParamEmpresa.ClaveEmpresa;
                                    loParamSubProceso.CveDatoEntrada = ""; //loDatoEntrada.Clave;
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

                                    loParamSubProceso.Usuario = ""; // loConfExtra.ClaveLogin

                                    loParamSubProceso.oServExtracDet.Rows[0][0] = ""; // loConfExtra.ClaveLogin
                                    loParamSubProceso.oServExtracDet.Rows[0][1] = ""; // loParamEmpresa.ClaveEmpresa
                                    loParamSubProceso.oServExtracDet.Rows[0][2] = ""; // Estado_T.ACTIVO
                                    loParamSubProceso.oServExtracDet.Rows[0][3] = ""; // ServExtractDet.lnEstusEvento.EnEjecucion
                                    loParamSubProceso.oServExtracDet.Rows[0][4] = ""; // Now.Date Fecha Alta
                                    loParamSubProceso.oServExtracDet.Rows[0][5] = ""; // Now.Date Fecha Evento
                                    loParamSubProceso.oServExtracDet.Rows[0][6] = ""; // New Date(1900, 1, 1, Now.Hour, Now.Minute, Now.Second) Hora Alta
                                    loParamSubProceso.oServExtracDet.Rows[0][7] = ""; // .GeneroAlarma = False
                                    loParamSubProceso.oServExtracDet.Rows[0][8] = ""; //.HoraEvento = New Date(1900, 1, 1,Now.Hour, Now.Minute, Now.Second)
                                    loParamSubProceso.oServExtracDet.Rows[0][9] = ""; //.IdAlarma = 0
                                    loParamSubProceso.oServExtracDet.Rows[0][10] = ""; //.IdDatosEntrada = loDatoEntrada.RowNumber
                                    loParamSubProceso.oServExtracDet.Rows[0][11] = ""; //.IdEmpresa = loParamEmpresa.IdEmpresa
                                    loParamSubProceso.oServExtracDet.Rows[0][12] = ""; //.IdServExt = loServExtrac.RowNumber
                                    loParamSubProceso.oServExtracDet.Rows[0][13] = ""; //.Mensaje = String.Empty
                                    loParamSubProceso.oServExtracDet.Rows[0][14] = ""; //.NumEjercicio = loParamSubProceso.Ejercicio
                                    loParamSubProceso.oServExtracDet.Rows[0][15] = ""; //.NumPeriodo = loParamSubProceso.Periodo
                                    
                                    Colhilos[hilo].Start(loParamSubProceso);
                                    break;
                                }
                            }                         
                               
                        } while (true) ;
                }
                }               

            }
            catch (OracleException ex)
            {
                validarError(ex.Code, ex.Message, "", ex.Message);
            }
            catch (Exception ex)
            {
                int liIdConfExtraccion = 0;
                if (loConfExtra != null)
                    //liIdConfExtraccion = loConfExtra.RowNumber;

                validarError(0, "Error al ejecutar proceso. " + ex.Message, "", ex.Message);

                //EnviaCorreo("Proceso extracción","Se presentó una falla inesperada en el proceso de extraccion.\n"
                // + "IdConfExtraccion=" + liIdConfExtraccion.ToString + "\n" +
                // "Motivo:" + lsMensaje, true);
            }
            finally
            {
                finalizaExtrac(int.Parse(toIdConfExtraccion.ToString()));
            }    
        }

        private void EjecutaSubProceso(object toParamSubProceso)
        {

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

            try
            {
                lsCadenaCon = getConnString(lsAmbiente);
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

            try
            {
                //Validamos las conexiones a FiscalX
                lsAmbiente = "ORAPREQA";
                lsCadenaCon = getConnString(lsAmbiente);
                OracleConnection loConexion = new OracleConnection(lsCadenaCon);

                loConexion.Open();
                loConexion.Close();

                // Validamos las conexiones a TV
                lsAmbiente = "TVQA";
                lsCadenaCon = getConnString(lsAmbiente);
                loConexion = new OracleConnection(lsCadenaCon);

                loConexion.Open();
                loConexion.Close();

                // Validamos las conexiones a TVMEX
                lsAmbiente = "TVMEXQA";
                lsCadenaCon = getConnString(lsAmbiente);
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

        public string getConnString(string tsAmbiente)
        {
            string lstrServidor = string.Empty;
            string lstrEsta = string.Empty;
            string lsrtLine = string.Empty;
            string lstrHost = string.Empty;
            string lstrPort = string.Empty;
            string lstrUser = string.Empty;
            string lstrPass = string.Empty;
            string lstrConn = string.Empty;
            string lsNombreArchivo = "";

            StreamReader loReader;

            if (File.Exists(lsNombreArchivo))
                loReader = new StreamReader(lsNombreArchivo);
            else
            {
                lsNombreArchivo = "";

                if (File.Exists(lsNombreArchivo))
                    loReader = new StreamReader(lsNombreArchivo);
                else
                    throw new ApplicationException("No se encontró el archivo de conexiones en la ruta: " + lsNombreArchivo);
            }

            int lintNum = 0;

            do
            {
                lsrtLine = loReader.ReadLine();
                if (!string.IsNullOrEmpty(lsrtLine))
                {
                    string[] words = lsrtLine.Split(new char[] { '|' });
                    if (words.Length == 7)
                    {
                        lstrEsta = words[6].Trim();
                        lstrServidor = words[1].Trim();

                        if (lstrEsta == "ACTIVO" && tsAmbiente == lstrServidor)
                        {
                            lintNum += 1;

                            lstrHost = words[2].Trim();
                            lstrPort = words[3].Trim();
                            lstrUser = words[4].Trim();
                            lstrPass = words[5].Trim();

                            break;
                        }
                    }
                }
            }
            while (!string.IsNullOrEmpty(lsrtLine));

            if (lintNum == 0)
            {
                throw new ApplicationException("No se encontró ninguna cadena Activa para el ambiente "
                        + tsAmbiente + ". Revisar validar con el Administrador de Conexiones.");
            }
            else if (lintNum == 1)
            {
                lstrConn = "SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + lstrHost + ")(PORT=" + lstrPort + "))(CONNECT_DATA=(SERVICE_NAME=" + lstrServidor + ")));uid=" + lstrUser + "; pwd=" + lstrPass + "";
            }
            else
            {
                lstrConn = string.Empty;
                throw new ApplicationException("Se encontró más de una cadena de conexión activa."
                                            + " Favor de Validar con el Administrador de Conexiones");
            }

            loReader.Close();

            return lstrConn;
        }

        public static string Decrypt_sha(string cipherText)
            {
                string passPhrase = "KAaz20*50";
                string saltValue = "SifitT_7";
                string hashAlgorithm = "SHA1";

                int passwordIterations = 2;
                string initVector = "@1B2c3D4e5F6g7H8";
                int keySize = 256;
                // Convert strings defining encryption key characteristics into byte
                // arrays. Let us assume that strings only contain ASCII codes.
                // If strings include Unicode characters, use Unicode, UTF7, or UTF8
                // encoding.
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

                // Convert our ciphertext into a byte array.
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

                // First, we must create a password, from which the key will be 
                // derived. This password will be generated from the specified 
                // passphrase and salt value. The password will be created using
                // the specified hash algorithm. Password creation can be done in
                // several iterations.
                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

                // Use the password to generate pseudo-random bytes for the encryption
                // key. Specify the size of the key in bytes (instead of bits).
                byte[] keyBytes = password.GetBytes(keySize / 8);

                // Create uninitialized Rijndael encryption object.
                RijndaelManaged symmetricKey = new RijndaelManaged();

                // It is reasonable to set encryption mode to Cipher Block Chaining
                // (CBC). Use default options for other symmetric key parameters.
                symmetricKey.Mode = CipherMode.CBC;

                // Generate decryptor from the existing key bytes and initialization 
                // vector. Key size will be defined based on the number of the key 
                // bytes.
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

                // Define memory stream which will be used to hold encrypted data.
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

                // Define cryptographic stream (always use Read mode for encryption).
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                // Since at this point we don't know what the size of decrypted data
                // will be, allocate the buffer long enough to hold ciphertext;
                // plaintext is never longer than ciphertext.
                byte[] plainTextBytes = new byte[cipherTextBytes.Length - 1 + 1];

                // Start decrypting.
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                // Close both streams.
                memoryStream.Close();
                cryptoStream.Close();

                // Convert decrypted data into a string. 
                // Let us assume that the original plaintext string was UTF8-encoded.
                string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                // Return decrypted string.   
                return plainText;
            }       
    }
}
