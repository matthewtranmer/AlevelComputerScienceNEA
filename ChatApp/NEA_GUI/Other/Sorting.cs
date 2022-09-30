

namespace NEA_GUI
{
    public static class Sorting
    {
        private static Dictionary<char, int> alphabet = new Dictionary<char, int>()
        {
            { 'a', 1 },
            { 'b', 2 },
            { 'c', 3 },
            { 'd', 4 },
            { 'e', 5 },
            { 'f', 6 },
            { 'g', 7 },
            { 'h', 8 },
            { 'i', 9 },
            { 'j', 10 },
            { 'k', 11 },
            { 'l', 12 },
            { 'm', 13 },
            { 'n', 14 },
            { 'o', 15 },
            { 'p', 16 },
            { 'q', 17 },
            { 'r', 18 },
            { 's', 19 },
            { 't', 20 },
            { 'u', 21 },
            { 'v', 22 },
            { 'w', 23 },
            { 'x', 24 },
            { 'y', 25 },
            { 'z', 26 }
        };

        public static List<MainForm.Message> RecursiveMergeSortAtoZ(List<MainForm.Message> unsorted_messages)
        {
            MainForm.Message[] messages = unsorted_messages.ToArray();
            MainForm.Message[] sorted_array = RecursiveMergeSortAtoZHelper(messages);

            List<MainForm.Message> sorted_list = new List<MainForm.Message>();
            foreach (var i in sorted_array)
            {
                sorted_list.Add(i);
            }

            return sorted_list;
        }

        private static MainForm.Message[] RecursiveMergeSortAtoZHelper(MainForm.Message[] array)
        {
            if (array.Length <= 1)
            {
                return array;
            }

            int midpoint = array.Length / 2;

            MainForm.Message[] left_copy = new MainForm.Message[midpoint];
            MainForm.Message[] right_copy = new MainForm.Message[array.Length - midpoint];

            Array.Copy(array, left_copy, midpoint);
            Array.Copy(array, midpoint, right_copy, 0, right_copy.Length);

            MainForm.Message[] left = RecursiveMergeSortAtoZHelper(left_copy);
            MainForm.Message[] right = RecursiveMergeSortAtoZHelper(right_copy);

            MainForm.Message[] sorted = new MainForm.Message[array.Length];

            int a = 0;
            int b = 0;
            int c = 0;

            while (a < left.Length && b < right.Length)
            {
                char left_char = char.ToLower(left[a].username[0]);
                char right_char = char.ToLower(right[b].username[0]);

                if (alphabet[left_char] < alphabet[right_char])
                {
                    sorted[c++] = left[a++];
                    continue;
                }
                sorted[c++] = right[b++];
            }

            while (a < left.Length)
            {
                sorted[c++] = left[a++];
            }

            while (b < right.Length)
            {
                sorted[c++] = right[b++];
            }

            return sorted;
        }


        public static List<MainForm.Message> RecursiveMergeSortOldestMessage(List<MainForm.Message> unsorted_messages)
        {
            MainForm.Message[] messages = unsorted_messages.ToArray();
            MainForm.Message[] sorted_array = RecursiveMergeSortOldestMessageHelper(messages);

            List<MainForm.Message> sorted_list = new List<MainForm.Message>();
            foreach (var i in sorted_array)
            {
                sorted_list.Add(i);
            }

            return sorted_list;
        }

        private static MainForm.Message[] RecursiveMergeSortOldestMessageHelper(MainForm.Message[] array)
        {
            if (array.Length <= 1)
            {
                return array;
            }

            int midpoint = array.Length / 2;

            MainForm.Message[] left_copy = new MainForm.Message[midpoint];
            MainForm.Message[] right_copy = new MainForm.Message[array.Length - midpoint];

            Array.Copy(array, left_copy, midpoint);
            Array.Copy(array, midpoint, right_copy, 0, right_copy.Length);

            MainForm.Message[] left = RecursiveMergeSortAtoZHelper(left_copy);
            MainForm.Message[] right = RecursiveMergeSortAtoZHelper(right_copy);

            MainForm.Message[] sorted = new MainForm.Message[array.Length];

            int a = 0;
            int b = 0;
            int c = 0;

            while (a < left.Length && b < right.Length)
            { 
                if (left[a].last_message < right[b].last_message)
                {
                    sorted[c++] = left[a++];
                    continue;
                }
                sorted[c++] = right[b++];
            }

            while (a < left.Length)
            {
                sorted[c++] = left[a++];
            }

            while (b < right.Length)
            {
                sorted[c++] = right[b++];
            }

            return sorted;
        }
    }
}