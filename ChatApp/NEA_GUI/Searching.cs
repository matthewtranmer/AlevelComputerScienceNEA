using System;

public static class Sorting
{
	public static List<Message> RecursiveMergeSortAtoZ(List<Message> unsorted_messages)
	{
		Message[] messages = unsorted_messages.ToArray();

	}

	private static Message[] RecursiveMergeSortAtoZHelper(Message[] messages)
	{
		if (messages.Length <= 1)
		{
			return messages;
		}

		int length = messages.Length;
		int left_length = messages.Length / 2;
		int right_length = length - left_length;

		Message[] left = new Message[left_length];
		Array.Copy(messages, left, left_length);

		Message[] right = new Message[right_length];
		Array.Copy(messages, left_length, right, 0, right_length);


    }
}
