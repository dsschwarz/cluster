using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace cluster
{
    class ClusterGraphService
    {
        private readonly NodeCollection _nodeCollection;

        public ClusterGraphService()
        {
            _nodeCollection = new NodeCollection();
        }

        public void UserLoadWebPage(string url)
        {
//            LoadWebPage(url, userTriggered: true);
//            var initialMatrix = CreateMatrix();
            var cachedString = @"1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1
1 1 1 0 0 0 0 1 1 0 0 0 0 0 1 0 0 0 1 0 0 1 1 0 1 1
1 1 1 0 1 0 1 1 1 0 0 0 0 1 1 0 0 0 1 1 1 1 1 0 1 1
1 0 0 1 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 1 1
1 0 1 0 1 0 0 0 0 1 0 0 0 0 0 0 0 1 0 0 0 0 1 0 1 1
1 0 0 0 0 1 0 1 0 0 0 0 0 0 1 0 0 0 0 0 0 0 1 0 1 1
1 0 1 0 0 0 1 0 1 1 0 0 0 0 0 0 0 1 0 0 0 0 1 0 1 1
1 1 1 0 0 1 0 1 1 0 0 0 0 0 1 0 0 0 0 0 0 1 1 0 1 1
1 1 1 1 0 0 1 1 1 1 0 0 1 1 0 1 0 0 0 1 1 1 1 0 1 1
1 0 0 0 1 0 1 0 1 1 0 0 0 0 0 0 0 1 0 0 0 0 1 0 1 1
1 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 1 0 1 1
1 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 1 0 1 1
1 0 0 0 0 0 0 0 1 0 0 0 1 0 0 0 0 0 0 0 0 0 1 0 1 1
1 0 1 0 0 0 0 0 1 0 0 0 0 1 0 0 0 0 0 0 0 1 1 0 1 1
1 1 1 0 0 1 0 1 0 0 0 0 0 0 1 0 0 0 0 0 0 1 1 0 1 1
1 0 0 0 0 0 0 0 1 0 0 0 0 0 0 1 0 0 0 0 0 0 1 0 1 1
1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 1 0 1 1
1 0 0 0 1 0 1 0 0 1 0 0 0 0 0 0 0 1 0 0 0 0 1 0 1 1
1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 0 1 1
1 0 1 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 1 0 0 1 0 1 1
1 0 1 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 1 1 1 0 1 1
1 1 1 0 0 0 0 1 1 0 0 0 0 1 1 0 0 0 0 0 1 1 1 0 1 1
1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 1 1
1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 1
1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 1 1
1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1";
            double[,] initialMatrix = ParseStringToMatrix(@"0 .25 .33 .33 0 0 0
.33 0 .33 .33 .33 0 0
.33 .25 0 .33 0 0 0
.33 .25 .33 0 0 0 0
0 .25 0 0 0 .5 .5
0 0 0 0 .33 0 .5
0 0 0 0 .33 .5 0");
            var clusteredMatrix = ClusterMatrix(initialMatrix);
        }

        private double[,] ParseStringToMatrix(string matrixString)
        {
            var rows = matrixString.Split('\n');
            var splitRows = rows.Select(row => row.Split(' ')).ToList();
            var matrix = new double[splitRows.Count, splitRows.First().Length];
            for (var i = 0; i < splitRows.Count; i++)
            {
                var row = splitRows[i];
                for (var j = 0; j < row.Length; j++)
                {
                    matrix[i, j] = double.Parse(row[j]);
                }
            }
            return matrix;
        }

        private double[,] ClusterMatrix(double[,] initialMatrix)
        {
            double[,] lastMatrix;
            var currentMatrix = initialMatrix;
            var repetitions = 0;
            do
            {
                repetitions += 1;
                lastMatrix = currentMatrix;
                currentMatrix = ExpandMatrix(currentMatrix);
                currentMatrix = InflateMatrix(currentMatrix);
                currentMatrix = NormalizeMatrix(currentMatrix);

            } while (!CompareMatrix(lastMatrix, currentMatrix));
            OutputMatrix(currentMatrix);
            Console.WriteLine(repetitions + " repetitions");
            return currentMatrix;
        }

        /// <summary>
        /// Scales all columns to sum to 1
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private double[,] NormalizeMatrix(double[,] matrix)
        {
            var newMatrix = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (var col = 0; col < matrix.GetLength(0); col++)
            {
                double columnSum = 0;
                for (var row = 0; row < matrix.GetLength(1); row++)
                {
                    columnSum += matrix[row, col];
                }
                for (var row = 0; row < matrix.GetLength(1); row++)
                {
                    newMatrix[row, col] = matrix[row, col]/columnSum;
                }
            }
            return newMatrix;
        }

        private bool CompareMatrix(double[,] lastMatrix, double[,] currentMatrix)
        {
            if (lastMatrix.GetLength(0) != currentMatrix.GetLength(0)) return false;
            if (lastMatrix.GetLength(1) != currentMatrix.GetLength(1)) return false;
            for (var i = 0; i < lastMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < lastMatrix.GetLength(0); j++)
                {
                    var lastValue = lastMatrix[i, j];
                    var currentValue = currentMatrix[i, j];
                    double allowedError = Math.Max(Math.Abs(lastValue), Math.Abs(currentValue)) * .00001;
                    if (Math.Abs(lastValue - currentValue) > allowedError)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private double[,] ExpandMatrix(double[,] matrix, int pow=2)
        {
            int rowCount = matrix.GetLength(0);
            int columnCount = matrix.GetLength(1);
            if (rowCount != columnCount) throw new Exception("row and column count are different: " + rowCount + ", " + columnCount);

            int size = rowCount;
            var newMatrix = new double[size, size];
            pow.Times(() =>
            {
                for (var row = 0; row < size; row++)
                {
                    for (var col = 0; col < size; col++)
                    {
                        double sum = 0;
                        for (var index = 0; index < size; index++)
                        {
                            sum += matrix[row, index]*matrix[index, col];
                        }
                        newMatrix[row, col] = sum;
                    }
                }
            });
            return newMatrix;
        }

        private double[,] InflateMatrix(double[,] matrix, double inflationParam = 2)
        {
            int rowCount = matrix.GetLength(0);
            int columnCount = matrix.GetLength(1);

            var newMatrix = new double[rowCount, columnCount];
            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < columnCount; col++)
                {
                    newMatrix[row, col] = Math.Pow(matrix[row, col], inflationParam);
                }
            }
            return newMatrix;
        }

        public void LoadWebPage(string url, bool userTriggered=false)
        {
            var absoluteLinks = WebService.FetchLinks(url);
            Console.WriteLine("------------------------");
            Console.WriteLine("Parsing " + url);
            var enumerable = absoluteLinks as IList<string> ?? absoluteLinks.ToList();
            Console.WriteLine("Absolute links: " + enumerable.Count);

            if (!_nodeCollection.NodeExists(url)) _nodeCollection.CreateNode(url);

            foreach (var absoluteLink in enumerable)
            {
                _nodeCollection.AddUrlLink(url, absoluteLink);
                // load every page that does not already exist in collection
                if (userTriggered && !_nodeCollection.NodeExists(absoluteLink))
                {
                    LoadWebPage(absoluteLink, userTriggered: false);
                }
            }
        }

        private double[,] CreateMatrix()
        {
            var nodes = _nodeCollection.GetNodes();
            var size = nodes.Count;
            var matrix = new double[size, size];
            const int selfReferentialValue = 1;

            /*
            Traverse like:
            1 n n
            c 1 n
            c c 1

            Where n is a calculated value, c is a copy of n, and 1 is 1 and all alone and ever more shall be so
            */
            for (var i = 0; i < size; i++)
            {
                var rowUrl = nodes[i].GetUrl();
                matrix[i, i] = selfReferentialValue;
                for (var j = i+1; j < size; j++)
                {
                    var columnUrl = nodes[j].GetUrl();
                    var weight = _nodeCollection.GetWeight(rowUrl, columnUrl);
                    matrix[i, j] = weight;
                    matrix[j, i] = weight;
                }
            }
            Console.WriteLine("Matrix computed");
            OutputMatrix(matrix);
            return matrix;
        }

        private void OutputLinks(IEnumerable<string> links)
        {
            foreach (var link in links)
            {
                Console.WriteLine(link);
            }
        }

        private void OutputMatrix(double[,] matrix)
        {
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(" " + matrix[i, j]);
                }
                Console.WriteLine("\n");
            }
        }
    }
}
