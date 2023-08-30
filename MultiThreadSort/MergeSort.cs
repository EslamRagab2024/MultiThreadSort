using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace MultiThreadSort
{
    public class MergeSort
    {
        #region Helper Functions [TASK 1]
        public static object Params2Object(int[] A, int s, int e, int m, int node_idx)
        {
            #region [TASK 1.1]
            //TODO: Encapsulate the given params into a single ArrayList object, then return it
            ArrayList parameters = new ArrayList();
            parameters.Add(A);
            parameters.Add(s);
            parameters.Add(e);
            parameters.Add(m);
            parameters.Add(node_idx);
            return parameters;
            #endregion
        }

        public static void Object2Params(object parameters, ref int[] A, ref int s, ref int e, ref int m, ref int node_idx)
        {
            #region [TASK 1.2]
            //TODO: Extract ALL params from the given ArrayList object "parameters", then store each of them in the corresponding "ref" variable 
            ArrayList myparameter = (ArrayList)parameters;
            A = (int[])myparameter[0];
            s = (int)myparameter[1];
            e = (int)myparameter[2];
            m = (int)myparameter[3];
            node_idx = (int)myparameter[4];
            #endregion
        }
        #endregion

        //DO NOT CHANGE THIS CODE
        #region Sequential Sort 

        public static void Sort(int[] array)
        {
            MSort(array, 1, array.Length);
        }

        private static void MSort(int[] A, int s, int e)
        {
            if (s >= e)
            {
                return;
            }

            int m = (s + e) / 2;

            MSort(A, s, m);

            MSort(A, m + 1, e);

            Merge(A, s, m, e);
        }

        private static void Merge(int[] A, int s, int m, int e)
        {
            int leftCapacity = m - s + 1;

            int rightCapacity = e - m;

            int leftIndex = 0;

            int rightIndex = 0;

            int[] Left = new int[leftCapacity];

            int[] Right = new int[rightCapacity];

            for (int i = 0; i < Left.Length; i++)
            {
                Left[i] = A[s + i - 1];
            }

            for (int j = 0; j < Right.Length; j++)
            {
                Right[j] = A[m + j];
            }

            for (int k = s; k <= e; k++)
            {
                if (leftIndex < leftCapacity && rightIndex < rightCapacity)
                {
                    if (Left[leftIndex] < Right[rightIndex])
                    {
                        A[k - 1] = Left[leftIndex++];
                    }
                    else
                    {
                        A[k - 1] = Right[rightIndex++];
                    }
                }
                else if (leftIndex < leftCapacity)
                {
                    A[k - 1] = Left[leftIndex++];
                }
                else
                {
                    A[k - 1] = Right[rightIndex++];
                }
            }
        }
        #endregion

        //TODO: Change this function to be MULTITHREADED
        //HINT: Remember to handle any dependency and/or critical section issues

        #region Multithreaded Sort [REMAINING TASKS]
        static int NumMergeSortThreads;
        static int pass;

        #region Semaphores
        //TODO: Define any required semaphore here
        static Semaphore require, require2, require3, finish;
        static Semaphore[] mysemaphore;
        #endregion

        #region Threads
        //TODO: Define any required thread objects here
        static Thread MS1, MS2, MS3, MS4, MT, MT2, MT3;
        static Thread[] threadsort, threadmerge;
        #endregion

        #region Sort Function
        public static void SortMT(int[] array)
        {
            int s = 1;
            int e = array.Length;
            int m = (s + e) / 2;
            int node_idx = 0;
            //NumMergeSortThreads = 2;                //TASK 2
            NumMergeSortThreads = 4;              //TASK 3
            //NumMergeSortThreads = 64;              //TASK 4  any number is 2 power 

            pass = NumMergeSortThreads / 2;

            #region [TASK 2]
            if (NumMergeSortThreads == 2)       //TASK 2
            {
                /*TODO: 
                 * 1) Initialize any required semaphore
                 * 2) Create & Start TWO Merge Sort concurrent threads & ONE Merge thread
                 * 3) Use semaphores to handle any dependency or critical section
                 */
                //object parameter=Params2Object(array,s,e,m,node_idx);
                //MSortMT(parameters);
                require = new Semaphore(0);
                finish = new Semaphore(0);
                object parameter1 = Params2Object(array, s, m, (s + m) / 2, 0);
                object parameter2 = Params2Object(array, m + 1, e, (m + 1 + e) / 2, 0);
                MS1 = new Thread(MSortMT);
                MS2 = new Thread(MSortMT);
                MS1.Start(parameter1);
                MS2.Start(parameter2);
                object parameter = Params2Object(array, s, e, m, 0);
                MT = new Thread(MergeMT);
                MT.Start(parameter);
                finish.Wait();
            }
            #endregion

            #region [TASK 3]
            else if (NumMergeSortThreads == 4)   //TASK 3
            {
                /*TODO: 
                 * 1) Initialize any required semaphore
                 * 2) Create & Start TWO Merge Sort concurrent threads & ONE Merge thread
                 * 3) Use semaphores to handle any dependency or critical section
                 */

                require = new Semaphore(0);
                require2 = new Semaphore(0);
                require3 = new Semaphore(0);
                finish = new Semaphore(0);
                int e1 = e / 4;
                int e2 = m;
                int e3 = (3 * e) / 4;
                int e4 = e;
                object parameter1 = Params2Object(array, s, e1, (s + e1) / 2, 1);
                object parameter2 = Params2Object(array, e1 + 1, m, (e1 + 1 + m) / 2, 1);
                object parameter3 = Params2Object(array, e2 + 1, e3, (e2 + 1 + e3) / 2, 2);
                object parameter4 = Params2Object(array, e3 + 1, e, (e3 + 1 + e) / 2, 2);
                MS1 = new Thread(MSortMT);
                MS2 = new Thread(MSortMT);
                MS3 = new Thread(MSortMT);
                MS4 = new Thread(MSortMT);
                MS1.Start(parameter1);
                MS2.Start(parameter2);
                MS3.Start(parameter3);
                MS4.Start(parameter4);
                object par1 = Params2Object(array, s, e, m, 1);
                object par2 = Params2Object(array, s, m, (s + m) / 2, 2);
                object par3 = Params2Object(array, m + 1, e, (m + 1 + e) / 2, 3);
                MT = new Thread(MergeMT);
                MT2 = new Thread(MergeMT);
                MT3 = new Thread(MergeMT);
                MT.Start(par1);
                MT2.Start(par2);
                MT3.Start(par3);
                finish.Wait();
            }

            #endregion

            #region [TASK 4]
            else
            {
                finish = new Semaphore(0);
                threadsort = new Thread[NumMergeSortThreads + 1];
                threadmerge = new Thread[NumMergeSortThreads ];
                mysemaphore = new Semaphore[NumMergeSortThreads ];

                int partsize = e / NumMergeSortThreads;
                for (int i = NumMergeSortThreads / 2, cnt = 1; i > 0; i = i / 2, cnt = cnt * 2)
                {
                    for (int j = 0; j < i; j++)
                    {
                        mysemaphore[i + j] = new Semaphore(0);
                        int start = 1 + j * (partsize * 2 * cnt);
                        int end = (j == i - 1) ? e : (j + 1) * (partsize * 2 * cnt);
                        int mid = (start + end) / 2;
                        object parameter1 = Params2Object(array, start, end, mid, i + j);
                        threadmerge[i + j] = new Thread(MergeMT);
                        threadmerge[i + j].Start(parameter1);

                    }
                }

                //int cnt = 0;
                for (int i = 0; i < NumMergeSortThreads; i++)
                {
                    int startIdx = s + i * partsize;
                    int endIdx = (i == NumMergeSortThreads - 1) ? array.Length : (i + 1) * partsize;
                    int mid = (startIdx + endIdx) / 2;
                    object parameter = Params2Object(array, startIdx, endIdx, mid, i / 2);
                    threadsort[i] = new Thread(MSortMT);
                    threadsort[i].Start(parameter);
                }

                finish.Wait();
            }
            #endregion
        }

        private static void MSortMT(object parameters)
        {
            #region Extract params from the given object 
            int[] A = null;
            int s = 0;
            int e = 0;
            int m = 0;
            int node_idx = 0;
            Object2Params(parameters, ref A, ref s, ref e, ref m, ref node_idx);
            #endregion


            MSort(A, s, e);

            #region [TASK 2] 
            if (NumMergeSortThreads == 2)       //TASK 2
            {
                //TODO: Use semaphores to handle any dependency or critical section
                require.Signal();
            }
            #endregion

            #region [TASK 3]
            else if (NumMergeSortThreads == 4)   //TASK 3
            {
                //TODO: Use semaphores to handle any dependency or critical section
                if (node_idx % 2 == 0)
                { require3.Signal(); }
                else { require2.Signal(); }

            }
            #endregion
            #region [TASK4]
            else
            {
                mysemaphore[node_idx + pass].Signal();
            }
            #endregion
        }

        private static void MergeMT(object parameters)
        {
            #region Extract params from the given object
            int[] A = null;
            int s = 0;
            int e = 0;
            int m = 0;
            int node_idx = 0;
            Object2Params(parameters, ref A, ref s, ref e, ref m, ref node_idx);
            #endregion

            #region [TASK 2]
            if (NumMergeSortThreads == 2)       //TASK 2
            {
                //TODO: Use semaphores to handle any dependency or critical section
                require.Wait();
                require.Wait();
                Merge(A, s, m, e);
                finish.Signal();
            }
            #endregion

            #region [TASK 3]
            else if (NumMergeSortThreads == 4)   //TASK 3
            {
                //TODO: Use semaphores to handle any dependency or critical section
                if (node_idx == 2)
                {
                    require2.Wait();
                    require2.Wait();
                    Merge(A, s, m, e);
                    require.Signal();

                }
                else if (node_idx == 3)
                {
                    require3.Wait();
                    require3.Wait();
                    Merge(A, s, m, e);
                    require.Signal();
                }
                else if (node_idx == 1)
                {
                    require.Wait();
                    require.Wait();
                    Merge(A, s, m, e);
                    finish.Signal();
                }


            }

            #endregion
            #region [TASK4]
            else
            {
                mysemaphore[node_idx].Wait();
                mysemaphore[node_idx].Wait();
                Merge(A, s, m, e);
                int x = node_idx / 2;
                if (node_idx == 1)
                {
                    finish.Signal();
                }
                else
                {
                    mysemaphore[x].Signal();
                }
            }
            #endregion
        }
        #endregion

        #endregion

    }
}
