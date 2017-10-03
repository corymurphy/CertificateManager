using System.Collections.Generic;
using System.Text;

namespace CertificateManager.Logic
{
    public class DnsValidation
    {

        public bool ValidateDnsName(string name)
        {
            /*
            A to Z ; upper case characters
            a to z ; lower case characters
            0 to 9 ; numeric characters 0 to 9
            -      ; dash
            *      ; wildcard only if first item
            */
            if (string.IsNullOrWhiteSpace(name))
                return false;

            char[] chrArr = name.ToCharArray();

            int index = 0;
            foreach (char chr in chrArr)
            {

                if (!(char.IsLetterOrDigit(chr) || chr == '.' || chr == '-'))
                    if (!(index == 0 && chr == '*'))
                        return false;
                index++;
            }
            return true;
        }

        /// <summary>
        /// The maximum size of the extension attribute for the subject alternative name of 4bytes
        /// the maximum size for a single dns entry is 253bytes
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public bool IsSubjectAlternativeNameValid(string[] names)
        {
            if (names == null)
                return false;

            
            if (names.Length > 2048)
                return false;


            List<byte> totalSanSize = new List<byte>();


            foreach (string name in names)
            {
                if (string.IsNullOrWhiteSpace(name))
                    return false;

                byte[] nameBytes = Encoding.UTF8.GetBytes(name);

                if (nameBytes.Length > 253)
                    return false;

                if (!ValidateDnsName(name))
                    return false;


                foreach(byte nameByte in nameBytes)
                {
                    totalSanSize.Add(nameByte);

                    if (totalSanSize.Count > 4000)
                        return false;
                }

                if (totalSanSize.Count > 4000)
                    return false;

            }

            if (totalSanSize.Count > 4000)
                return false;

            return true;

        }
    }
}
