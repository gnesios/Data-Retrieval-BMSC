using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BMSCWebServiceReference
{
    public class ServiceConnector
    {
        #region Variables
        System.Net.NetworkCredential credentials;
        XmlNode nodeQuery;
        XmlNode nodeFields;
        XmlNode nodeOptions;

        string user;
        string pass;
        string domain;

        public string User
        {
            get { return user; }
            set { user = value; }
        }
        public string Pass
        {
            get { return pass; }
            set { pass = value; }
        }
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }
        #endregion

        #region Constructor methods
        /// <summary>
        /// Connector initializer method.
        /// </summary>
        /// <param name="user">Username with sharepoint permissions (reader at least)</param>
        /// <param name="pass">Password of the user name</param>
        /// <param name="domain">Network domain of the credentials</param>
        public ServiceConnector(string user, string pass, string domain)
        {
            User = user;
            Pass = pass;
            Domain = domain;

            credentials = new System.Net.NetworkCredential(User, Pass, Domain);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// BMSC web site service connector.
        /// </summary>
        /// <param name="list">Name of the sharepoint list</param>
        /// <param name="query">Query string (can be null or empty string)</param>
        /// <param name="fields">Array of fields to be queried (can be null)</param>
        /// <returns>String value of the XmlNode object.</returns>
        public string ConnectorBMSC(string list, string query, string[] fields)
        {
            BMSCWebReference.Lists listsBmsc = new BMSCWebReference.Lists();
            listsBmsc.Credentials = credentials;

            this.XmlNodeDefinitions(query, fields);

            try
            {
                XmlNode listItems = listsBmsc.GetListItems(
                    list, null, nodeQuery, nodeFields, null, nodeOptions, null);

                if (fields != null)
                {
                    XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
                    ns.AddNamespace("z", "#RowsetSchema");

                    XmlNodeList nodeList = listItems.SelectNodes("//z:row", ns);

                    foreach (XmlNode node in nodeList)
                    {//get all the rows
                        while (node.Attributes.Count > fields.Length)
                        {//remove unwanted attributes
                            node.Attributes.RemoveAt(node.Attributes.Count - 1);
                        }
                    }
                }

                return listItems.OuterXml;
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                return
                    "ERROR >> " + ex.Message +
                    "\nDETAIL >> " + ex.Detail.InnerText +
                    "\nTRACE >> " + ex.StackTrace;
            }
        }

        /// <summary>
        /// BANX web site service connector.
        /// </summary>
        /// <param name="list">Name of the sharepoint list</param>
        /// <param name="query">Query string (can be null or empty string)</param>
        /// <param name="fields">Array of fields to be queried (can be null)</param>
        /// <returns>String value of the XmlNode object.</returns>
        public string ConnectorBANX(string list, string query, string[] fields)
        {
            BANXWebReference.Lists listsBanx = new BANXWebReference.Lists();
            listsBanx.Credentials = credentials;

            this.XmlNodeDefinitions(query, fields);

            try
            {
                XmlNode listItems = listsBanx.GetListItems(
                    list, null, nodeQuery, nodeFields, null, nodeOptions, null);

                if (fields != null)
                {
                    XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
                    ns.AddNamespace("z", "#RowsetSchema");

                    XmlNodeList nodeList = listItems.SelectNodes("//z:row", ns);

                    foreach (XmlNode node in nodeList)
                    {//get all the rows
                        while (node.Attributes.Count > fields.Length)
                        {//remove unwanted attributes
                            node.Attributes.RemoveAt(node.Attributes.Count - 1);
                        }
                    }
                }

                return listItems.OuterXml;
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                return
                    "ERROR >> " + ex.Message +
                    "\nDETAIL >> " + ex.Detail.InnerText +
                    "\nTRACE >> " + ex.StackTrace;
            }
        }
        #endregion

        #region Private methods
        private void XmlNodeDefinitions(string query, string[] fields)
        {
            XmlDocument xmlDoc = new XmlDocument();
            nodeQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            nodeFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            nodeOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

            string formatedFields = "";
            try
            {//just in casse if fields is null
                for (int i = 0; i < fields.Length; i++)
                {
                    formatedFields += string.Format("<FieldRef Name='{0}' />", fields[i]);
                }
            }
            catch { }

            nodeOptions.InnerXml = "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>";
            nodeFields.InnerXml = formatedFields;
            nodeQuery.InnerXml = query;
        }
        #endregion
    }
}
