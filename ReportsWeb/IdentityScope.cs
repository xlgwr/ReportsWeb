using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;

using System.DirectoryServices;

namespace ReportsWeb
{
    public class syslogon {

        public static string ADValidate(string ADPath, string Uname, string Password)
        {

            DirectoryEntry de = new DirectoryEntry(ADPath, Uname, Password);
            try
            {
                DirectorySearcher search = new DirectorySearcher(de);
                search.Filter = "(sAMAccountName=" + Uname + ")";
                SearchResult result = search.FindOne();
                return "OK";
            }
            catch (Exception ex)
            {
                throw new Exception("ErrorAdValidate:Invalid UserName Or Password, Please try again.Thank your.");
            }
            finally
            {
                de.Close();
            }

        }

    
    }

    ///<summary>
    ///IdentityScope 的摘要说明
    ///</summary>
    public class IdentityScope : IDisposable
    {
        // obtains user token
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
        // closes open handes returned by LogonUser
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        extern static bool CloseHandle(IntPtr handle);
        [DllImport("Advapi32.DLL")]
        static extern bool ImpersonateLoggedOnUser(IntPtr hToken);
        [DllImport("Advapi32.DLL")]
        static extern bool RevertToSelf();
        // logon types

        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
        // logon providers

        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_PROVIDER_WINNT50 = 3;
        const int LOGON32_PROVIDER_WINNT40 = 2;
        const int LOGON32_PROVIDER_WINNT35 = 1;
        private bool disposed;

        ///<summary>
        /// 登录
        ///</summary>
        ///<param name="sUsername">用户名</param>
        ///<param name="sDomain">第二个参数是域名，有域名的话写域名，没有域名写目标机器的IP·</param>
        ///<param name="sPassword">密码</param>

        public static string IdentityScope_u(string sUsername, string sDomain, string sPassword)
        {
            // initialize tokens
            IntPtr pExistingTokenHandle = new IntPtr(0);
            IntPtr pDuplicateTokenHandle = new IntPtr(0);
            try
            {
                // get handle to token

                bool bImpersonated = LogonUser(sUsername, sDomain, sPassword, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref pExistingTokenHandle);
                //bool bImpersonated = LogonUser(sUsername, sDomain, sPassword, LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_DEFAULT, ref pExistingTokenHandle);
                if (true == bImpersonated)
                {
                    if (!ImpersonateLoggedOnUser(pExistingTokenHandle))
                    {
                        int nErrorCode = Marshal.GetLastWin32Error();

                        throw new Exception("ImpersonateLoggedOnUser error1;Code=" + nErrorCode);


                    }
                    return "OK";
                }
                else
                {
                    int nErrorCode = Marshal.GetLastWin32Error();
                    throw new Exception("UserName Or Password is Error;Code=" + nErrorCode);
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                // close handle(s)

                if (pExistingTokenHandle != IntPtr.Zero)
                    CloseHandle(pExistingTokenHandle);
                if (pDuplicateTokenHandle != IntPtr.Zero)
                    CloseHandle(pDuplicateTokenHandle);
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                RevertToSelf();
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}