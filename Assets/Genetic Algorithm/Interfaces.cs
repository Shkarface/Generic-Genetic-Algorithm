using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KurdifyEngine.GA
{
    public interface IConvertFromString<T>
    {
        T FromString(string content);
    }
    public interface ISinglePointCrossover<IConvertFromString>
    {
        IConvertFromString SinglePointCrossover(IConvertFromString Parent1, IConvertFromString Parent2, int mutationRatio);
    }
}