package com.dvt.kinderfinder.transmitter;

import java.util.LinkedList;

public class Utility {
	
	public static LinkedList<String> parseJSON(String json) {
		LinkedList<String> result = new LinkedList<String>();

		if (json.length() == 0)
			return result;

		// At this point: ["0,567178641556379","0,544309737726259"]
		/* Remove '[' and ']' from string. */
		if (json.charAt(0) == '[') {
			//json = json.Remove(json.length() - 1, 1);
			//json = json.Remove(0, 1);
			json = json.substring(1, json.length() - 1);
		}

		if (json.length() == 0)
			return result;

		String temp = "";

		// At this point: "0,567178641556379","0,544309737726259"
		for (int i = 0; i < json.length(); i++) {
			// Opening quotation found; read until closing one found:
			if (json.charAt(i) == '"') {
				while (json.charAt(++i) != '"')
					temp += json.charAt(i);
				// TODO: Possibly fix situation where there is a quotation mark embedded in the string, like:
				// ["hello","wo"rld"]
				// Check if the embedded quotation mark will be escaped automatically.
			}
			// Element separator found; add previous element:
			else if (json.charAt(i) == ',') {
				result.add(temp);
				temp = "";
			}
		}

		// Add final element:
		if (!temp.equals("")) {
			result.add(temp);
		}

		return result;
	}
}
