using System;

using Axiom.Component.OpenAsset;

namespace Axiom.Component.OpenAsset
{
    public enum FormatVersion
    {
        v1_5,
        v1_4,
        v1_3
    };

    public enum TransformType
    {
        LookAt,
        Rotate,
        Translate,
        Scale,
        Skew,
        Matrix
    };

    public enum InputType
    {
        Invalid,
        Vertex,  // special type for per-index data referring to the <vertices> element carrying the per-vertex data.
        Position,
        Normal,
        Texcoord,
        Color,
        Tangent,
        Bitangent
    };

    public enum UpDirection
    {
        X,
        Y,
        Z
    };

}
