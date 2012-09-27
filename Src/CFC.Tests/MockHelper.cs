using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CFC.Tests
{
    public class MockHelper
    {
        public MockHelper()
        {
            //this.MaxDepth = 100;
        }

        int index = 1;

        Dictionary<string, string> keys = new Dictionary<string, string>();
        List<string> assemblyList = new List<string>();
        StringBuilder codeBuilder = null;

        public int MaxIndexValue { get; set; }
        public string Generate(object obj)
        {
            //Assembly List that we are interested in for recursive calls, add according to your need.
            assemblyList.Add("SmartDataManagement.Blog");

            codeBuilder = new StringBuilder();
            GenerateCode(obj, string.Empty);
            return codeBuilder.ToString();
        }

        private void GenerateCode(object obj, string theKey)
        {
            string instanceName = string.Format("{0}{1}", obj.GetType().Name, index);
            codeBuilder.AppendLine(String.Format("{0} {1} = new {0}();", obj.GetType().Name, instanceName));
            keys.Add(theKey, instanceName);
            //Iterate through the list of Public Properties of the object instance
            foreach (System.Reflection.PropertyInfo property in obj.GetType().GetProperties())
            {
                //Decide to dig deeper, if a PropertyType belongs to the assemblies that we are interested in, we go deep.
                if (GoDeeper(property))
                {
                    index = index + 1;
                    //Avoid infinite Loop situation
                    if (index < this.MaxIndexValue)
                    {
                        object propertyInstance = property.GetValue(obj, null);
                        if (propertyInstance == null)
                        {
                            //Dynamically create Property Instance
                            propertyInstance = Activator.CreateInstance(property.PropertyType);
                            if (property.CanWrite)
                                property.SetValue(obj, propertyInstance, null);
                            string key = Guid.NewGuid().ToString();
                            codeBuilder.AppendLine();
                            //Recursive call
                            GenerateCode(propertyInstance, key);

                            codeBuilder.AppendLine(String.Format(@"{0}.{1} = {2};", instanceName, property.Name, keys[key]));
                            codeBuilder.AppendLine();
                        }
                    }
                }
                else if (property.PropertyType == typeof(string))
                {
                    codeBuilder.AppendLine(String.Format(@"{0}.{1} = ""{2}"";", instanceName, property.Name, GetRandomString()));
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    codeBuilder.AppendLine(String.Format(@"{0}.{1} = DateTime.Now;", instanceName, property.Name));
                }
                else if (property.PropertyType == typeof(int))
                {
                    codeBuilder.AppendLine(String.Format(@"{0}.{1} = {2};", instanceName, property.Name, GetRandomInt()));
                }
            }
        }

        private bool GoDeeper(PropertyInfo property)
        {
            //Dig deeper for interested Assemblies only, Please feel free to extend and put complex logics to serve your purpose.
            foreach (string assemblyName in assemblyList)
            {
                if (property.PropertyType.Assembly.FullName.Contains(assemblyName))
                    return true;
            }
            return false;
        }

        private string GetRandomString()
        {
            return String.Format("Random String {0}", Guid.NewGuid().ToString());
        }

        private int GetRandomInt()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt32(buffer, 0);
        }
    }
}
