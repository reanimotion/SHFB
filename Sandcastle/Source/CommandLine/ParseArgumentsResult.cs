// Copyright � Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.Ddue.Tools.CommandLine
{
    /// <summary>
    /// This is used to hold the results of parsing a set of command line option strings
    /// </summary>
    public sealed class ParseArgumentsResult
    {
        #region Private data members
        //=====================================================================

        private OptionCollection options;
        private Dictionary<string, ParseResult> errors;
        private List<string> nonOptions;

        #endregion

        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property is used to get the option collection related to the results
        /// </summary>
        public OptionCollection Options
        {
            get { return options; }
        }

        /// <summary>
        /// This read-only property is used to see if the options were parsed successfully
        /// </summary>
        /// <value>Returns true if successful, false if not</value>
        public bool Success
        {
            get { return (errors.Count == 0); }
        }

        /// <summary>
        /// This read-only property returns a collection of the unused arguments
        /// </summary>
        public ReadOnlyCollection<string> UnusedArguments
        {
            get { return new ReadOnlyCollection<string>(nonOptions); }
        }
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="options">The option collection related to the results</param>
        internal ParseArgumentsResult(OptionCollection options)
        {
            this.options = options;

            errors = new Dictionary<string, ParseResult>();
            nonOptions = new List<string>();
        }
        #endregion

        #region Helper methods
        //=====================================================================

        /// <summary>
        /// This is used to add a parsing error
        /// </summary>
        /// <param name="optionName">The option name</param>
        /// <param name="error">The error result</param>
        internal void AddError(string optionName, ParseResult error)
        {
            errors[optionName] = error;
        }

        /// <summary>
        /// This is used to add a non-option
        /// </summary>
        /// <param name="value">The non-option value</param>
        internal void AddNonOption(string value)
        {
            nonOptions.Add(value);
        }

        /// <summary>
        /// This is used to write out a list of all parsing errors
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to which the summary is written</param>
        public void WriteParseErrors(TextWriter writer)
        {
            string message;

            if(writer == null)
                throw new ArgumentNullException("writer");

            foreach(KeyValuePair<string, ParseResult> error in errors)
            {
                switch(error.Value)
                {
                    case ParseResult.ArgumentNotAllowed:
                        message = "The option argument is not allowed";
                        break;

                    case ParseResult.MalformedArgument:
                        message = "The option argument is malformed";
                        break;

                    case ParseResult.MissingOption:
                        // Use the message from the option
                        message = options[error.Key].RequiredMessage;
                        break;

                    case ParseResult.MultipleOccurence:
                        message = "The option cannot occur more than once";
                        break;

                    case ParseResult.UnrecognizedOption:
                        message = "Unrecognized option";
                        break;

                    default:
                        message = "Unexpected result code (" + error.Value.ToString() + ") for option";
                        break;
                }

                writer.WriteLine("{0}: {1}", message, error.Key);
            }
        }
        #endregion
    }
}
