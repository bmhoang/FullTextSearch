﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Protsyk.PMS.FullText.Core.Common.Compression
{
    public class HuffmanEncodingBuilder : VarLenCharEncodingBuilder
    {
        // TODO: Use heap
        private readonly SortedSet<TreeNode> nodes = new SortedSet<TreeNode>(
            Comparer<TreeNode>.Create((a, b) =>
             {
                 if (a.weight == b.weight)
                 {
                     return b.id - a.id;
                 }
                 return b.weight - a.weight;
             }
            ));

        protected override VarLenCharEncoding DoBuild()
        {
            foreach(var symbol in symbols)
            {
                nodes.Add(new CharNode(symbol.c, symbol.f));
            }

            while (nodes.Count > 1)
            {
                var node1 = nodes.Max;
                nodes.Remove(node1);

                var node2 = nodes.Max;
                nodes.Remove(node2);

                nodes.Add(new CombineNode(node1, node2));
            }

            return new HuffmanEncoding(nodes.Single());
        }

        struct HuffmanCode
        {
            public readonly char c;
            public readonly int code;
            public readonly int bitLength;

            public HuffmanCode(char c, int code, int bitLength)
            {
                this.c = c;
                this.code = code;
                this.bitLength = bitLength;
            }

            public string GetCode()
            {
                var str = new char[bitLength];
                var temp = code;
                for (int i=0; i<bitLength; ++i)
                {
                    str[bitLength - i - 1] = ((temp & 1) != 0) ? '1' : '0';
                    temp >>= 1;
                }
                return new string(str);
            }
        }

        class HuffmanEncoding : VarLenCharEncoding
        {
            public HuffmanEncoding(IEncodingNode root)
                : base(root)
            {
            }
        }

        class TreeNode : IEncodingNode
        {
            private static int nextId = 0;

            public readonly int id;
            public readonly int weight;

            public TreeNode(int weight)
            {
                this.weight = weight;
                this.id = Interlocked.Increment(ref nextId);
            }
        }

        class CharNode : TreeNode, IEncodingLeafNode
        {
            public readonly char c;

            public CharNode(char c, int weight)
                : base(weight)
            {
                this.c = c;
            }

            char IEncodingLeafNode.V => c;
        }

        class CombineNode : TreeNode, IEncodingTreeNode
        {
            public readonly TreeNode left;
            public readonly TreeNode right;

            public CombineNode(TreeNode left, TreeNode right)
                : base(left.weight + right.weight)
            {
                this.left = left;
                this.right = right;
            }

            IEncodingNode IEncodingTreeNode.Left => left;

            IEncodingNode IEncodingTreeNode.Right => right;
        }
    }
}
