using System.Collections.Generic;

namespace NDesk.Options.Extensions
{
    /// <summary>
    /// VariableMatrix OptionItemBase class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VariableMatrix<T> : OptionItemBase<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prototype"></param>
        public VariableMatrix(string prototype)
            : base(prototype)
        {
        }

        //TODO: Might could enrich the model by implementing our own custom Dictionary.
        /// <summary>
        /// Matrix backing field.
        /// </summary>
        private readonly Dictionary<string, T> _matrix = new Dictionary<string, T>();

        /// <summary>
        /// Gets the Matrix.
        /// </summary>
        public Dictionary<string, T> Matrix
        {
            get { return _matrix; }
        }
    }
}
