//MIT License
//
//Copyright(c) 2018 - 2022 nessie1980(nessie1980@gmx.de)
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parser
{
    /// <summary>
    /// Class of a regex list
    /// This class store the name and a RegexElement
    /// in a dictionary.
    /// The name is used for creating the result dictionary.
    /// </summary>
    [Serializable]
    public class RegExList
    {
        #region Properties

        public Dictionary<string, RegexElement> RegexListDictionary { set; get; }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Last thrown exception of the class
        /// </summary>
        public Exception LastException { get; private set; }

        #endregion Properties

        #region Methodes

        /// <summary>
        /// Constructor for building a RegExList instance
        /// </summary>
        public RegExList()
        {
            try
            {
                RegexListDictionary = new Dictionary<string, RegexElement>();
                LastException = null;
            }
            catch (Exception ex)
            {
                RegexListDictionary = null;
                LastException = ex;
            }
        }


        /// <summary>
        /// Constructor for building a RegExList instance
        /// </summary>
        /// <param name="name">Name of the regex. This name will be used for creating the result dictionary</param>
        /// <param name="regexElement">RegexElement with the regex search string and with the optional regex options</param>
        // ReSharper disable once UnusedMember.Global
        public RegExList(string name, RegexElement regexElement)
        {
            try
            {
                RegexListDictionary = new Dictionary<string, RegexElement>
                {
                    { name, regexElement }
                };

                LastException = null;
            }
            catch (Exception ex)
            {
                RegexListDictionary = null;
                LastException = ex;
            }
        }

        /// <summary>
        /// This function adds a new entry to the dictionary
        /// The return value indicates if the add was successful.
        /// If the add failed the value "LastException" stores the exception which had been occurred.
        /// </summary>
        /// <param name="name">Name of the regex expression. This name will be used for creating the result dictionary</param>
        /// <param name="regexElement">RegexElement with the regex search string and with the optional regex options</param>
        /// <returns>Flag if the add was successful </returns>
        /// true  = successful
        /// false = failed
        // ReSharper disable once UnusedMember.Global
        public bool Add(string name, RegexElement regexElement)
        {
            try
            {
                RegexListDictionary.Add(name, regexElement);

                return true;
            }
            catch (Exception ex)
            {
                LastException = ex;
                return false;
            }
        }

        #endregion Methodes
    }

    /// <summary>
    /// Class of a RegexElement
    /// This class stores the regex search string and
    /// if necessary a list of regex options
    /// </summary>
    [Serializable]
    public class RegexElement
    {
        #region Properties

        /// <summary>
        /// String with the regex expression
        /// </summary>
        public string RegexExpression { set; get; }

        /// <summary>
        /// List with the regex option for the search
        /// </summary>
        public List<RegexOptions> RegexOptions { set; get; }

        /// <summary>
        /// Flag if a parsing result can be empty
        /// </summary>
        public bool ResultEmpty { set; get; }

        /// <summary>
        /// Index of the found position
        /// </summary>
        public int RegexFoundPosition { set; get; }

        #endregion Properties

        #region Methodes

        /// <summary>
        /// Constructor for building a RegexElement instance
        /// </summary>
        /// <param name="regexExpression">The string for the search string of the regex</param>
        /// <param name="regexFoundPosition">The index of the found value</param>
        /// <param name="resultEmpty">Flag if the parsing result can be empty</param>
        /// <param name="regexOptions">The list with the regex options. This parameter is optional</param>
        public RegexElement(string regexExpression, int regexFoundPosition, bool resultEmpty, List<RegexOptions> regexOptions = null)
        {
            RegexExpression = regexExpression;
            RegexFoundPosition = regexFoundPosition;
            ResultEmpty = resultEmpty;
            RegexOptions = regexOptions;
        }

        #endregion Methodes
    }
}
