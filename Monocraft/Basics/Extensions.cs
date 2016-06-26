#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: Extensions.cs
// Date - created: 2016.06.22 - 10:56
// Date - current: 2016.06.25 - 18:38

#endregion

#region Usings

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Monocraft.Basics
{
    static class Extensions
    {
        public static BoundingBox BuildBoundingBox(this ModelMesh mesh, Matrix meshTransform)
        {
            // Create initial variables to hold min and max xyz values for the mesh
            var meshMax = new Vector3(float.MinValue);
            var meshMin = new Vector3(float.MaxValue);

            foreach (var part in mesh.MeshParts)
            {
                // The stride is how big, in bytes, one vertex is in the vertex buffer
                // We have to use this as we do not know the make up of the vertex
                var stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                var vertexData = new VertexPositionNormalTexture[part.NumVertices];
                part.VertexBuffer.GetData(part.VertexOffset*stride, vertexData, 0, part.NumVertices, stride);

                // Find minimum and maximum xyz values for this mesh part
                var vertPosition = new Vector3();

                for (var i = 0; i < vertexData.Length; i++)
                {
                    vertPosition = vertexData[i].Position;

                    // update our values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                }
            }

            // transform by mesh bone matrix
            meshMin = Vector3.Transform(meshMin, meshTransform);
            meshMax = Vector3.Transform(meshMax, meshTransform);

            // Create the bounding box
            var box = new BoundingBox(meshMin, meshMax);
            return box;
        }

        public static Texture2D CloneRenderTarget(this RenderTarget2D target, GraphicsDevice device)
        {
            var clone = new Texture2D(device, target.Width, target.Height);
            var tempArray = new Color[target.Width*target.Height];
            target.GetData(tempArray);
            clone.SetData(tempArray);

            return clone;
        }
    }
}