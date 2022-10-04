#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGame
{
    public abstract class BaseVariable
    {

    }

    [Serializable]
    public class Variable<T> : BaseVariable where T : unmanaged
    {
        [SerializeField]
        private T value;

        private List<Func<T, T>> processors = new();

        public T Value => value;

        public T ProcessedValue
        {
            get
            {
                T processedValue = value;
                foreach (Func<T, T> processor in processors)
                {
                    processedValue = processor(processedValue);
                }
                
                return processedValue;
            }
        }

        public Variable() { }

        public Variable(T value)
        {
            this.value = value;
        }

        public void AddProcessor(Func<T, T> processor)
        {
            processors.Add(processor);
        }

        public void RemoveProcessor(Func<T, T> processor)
        {
            processors.Remove(processor);
        }

        public static implicit operator Variable<T>(T value) => new(value);
        public static implicit operator T(Variable<T> variable) => variable.ProcessedValue;
    }
}