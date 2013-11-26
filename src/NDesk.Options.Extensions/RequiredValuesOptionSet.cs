using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NDesk.Options.Extensions
{
    /// <summary>
    /// Derives from OptionSet, and adds capability for variables that are required.
    /// </summary>
    /// <remarks>http://www.ndesk.org/doc/ndesk-options/NDesk.Options/OptionSet.html</remarks>
    public class RequiredValuesOptionSet : OptionSet
    {
        public IEnumerable<Option> GetMissingVariables()
        {
            // get items in dictionary where there is no entry
            var q = from t in _requiredVariableValues
                    join o in this as KeyedCollection<string, Option> on t.Key equals o.Prototype
                    where t.Value == false
                    select o;

            return q;
        }

        /// <summary>
        /// Dictionary that holds whether or not prototype variables have been set
        /// </summary>
        private readonly Dictionary<string, bool> _requiredVariableValues = new Dictionary<string, bool>();

        /// <summary>
        /// Adds the Required Variable to the OptionSet.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prototype"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public Variable<T> AddRequiredVariable<T>(string prototype, string description = null)
        {
            var variablePrototype = prototype + "=";

            _requiredVariableValues.Add(variablePrototype, false);

            var variable = new Variable<T>(variablePrototype);

            /* This is why a WET approach is bad, at best more-error-prone.
             * Also why I prefer a SOLID, DRY approach whenever possible. */
            Add(variablePrototype, description, x =>
            {
                variable.Value = Variable<T>.CastString(x);
                _requiredVariableValues[variablePrototype] = true;
            });

            return variable;
        }
    }
}
