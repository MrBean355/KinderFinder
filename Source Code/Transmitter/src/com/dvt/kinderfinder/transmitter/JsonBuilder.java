package com.dvt.kinderfinder.transmitter;

import java.util.AbstractMap.SimpleEntry;
import java.util.LinkedList;

public class JsonBuilder {
	private LinkedList<SimpleEntry<String, String>>	Entries	= new LinkedList<SimpleEntry<String, String>>();
	//private String output = "";
	
	public void addEntry(String key, String value) {
		Entries.add(new SimpleEntry<String, String>(key, value));
		//output += "\"" + key + "\":" + "\"" + value + "\",";
	}

	@Override
	public String toString() {
		String result = "{";

		for (SimpleEntry<String, String> entry : Entries) {
			result += "\"" + entry.getKey() + "\":";
			result += "\"" + entry.getValue() + "\",";
		}

		if (result.charAt(result.length() - 1) == ',') {
			result = result.substring(0, result.length() - 1);
		}

		result += '}';

		return result;
	}

	public static LinkedList<String> JsonToList(String json) {
		LinkedList<String> result = new LinkedList<String>();

		if (json.length() == 0) {
			return result;
		}

		// At this point: ["0,567178641556379","0,544309737726259"]
		/* Remove '[' and ']' from string. */
		if (json.charAt(0) == '[') {
			json = json.substring(1, json.length() - 1);
		}

		if (json.length() == 0) {
			return result;
		}

		String temp = "";

		// At this point: "0,567178641556379","0,544309737726259"
		for (int i = 0; i < json.length(); i++) {
			// Opening quotation found; read until closing one found:
			if (json.charAt(i) == '"') {
				while (json.charAt(++i) != '"') {
					temp += json.charAt(i);
				}
				// TODO: Possibly fix situation where there is a quotation mark
				// embedded in the string, like:
				// ["hello","wo"rld"]
				// Check if the embedded quotation mark will be escaped
				// automatically.
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
