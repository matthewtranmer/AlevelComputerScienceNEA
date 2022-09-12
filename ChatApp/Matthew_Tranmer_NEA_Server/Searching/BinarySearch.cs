using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Matthew_Tranmer_NEA_Server.Workers;

namespace Matthew_Tranmer_NEA.Searching
{
    internal static class BinarySearch
    {
        public static int searchTunnelWorker(List<ManagmentWorker> managment_tunnel_workers, int search_term)
        {
            return searchTunnelWorkerRecursive(managment_tunnel_workers, search_term, 0, managment_tunnel_workers.Count - 1);
        }

        //Not to be called. Call helper function instead.
        private static int searchTunnelWorkerRecursive(List<ManagmentWorker> managment_tunnel_workers, int UserID, int left_index, int right_index)
        {
            if (left_index > right_index)
            {
                return -1;
            }

            int mid_index = left_index + ((right_index - left_index) / 2);
            if (managment_tunnel_workers[mid_index].UserID == UserID)
            {
                return mid_index;
            }

            //left
            if (managment_tunnel_workers[mid_index].UserID > UserID)
            {
                return searchTunnelWorkerRecursive(managment_tunnel_workers, UserID, left_index, mid_index - 1);
            }

            //right
            return searchTunnelWorkerRecursive(managment_tunnel_workers, UserID, mid_index + 1, right_index);
        }

        public static void addTunnelWorker(List<ManagmentWorker> managment_tunnel_workers, ManagmentWorker worker)
        {
            for (int i = 0; i < managment_tunnel_workers.Count; i++)
            {
                if (managment_tunnel_workers[i].UserID > worker.UserID)
                {
                    managment_tunnel_workers.Insert(i, worker);
                    return;
                }
            }

            managment_tunnel_workers.Add(worker);
        }
    }
}
