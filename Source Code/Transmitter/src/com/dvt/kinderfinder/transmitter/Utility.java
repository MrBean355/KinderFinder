package com.dvt.kinderfinder.transmitter;

public class Utility {
	
	public static String removeQuotations(String input) {
		String result = "";
		
		for (int i = 0; i < input.length(); i++) {
			char ch = input.charAt(i);
			
			if (ch != '"') {
				result += ch;
			}
			else if (i > 0) {
				char prev = input.charAt(i - 1);
				
				if (prev == '\\') {
					result += ch;
				}
			}
		}
		
		return result;
	}
}
