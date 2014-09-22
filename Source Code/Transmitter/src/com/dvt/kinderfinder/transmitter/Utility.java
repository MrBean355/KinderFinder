package com.dvt.kinderfinder.transmitter;

import java.security.MessageDigest;

public class Utility {

	public static String hashPassword(String input) {
		try {
			String str = input + "2e6e76485b61254b2e73694d50";
			MessageDigest md = MessageDigest.getInstance("SHA-256");
			md.update(str.getBytes("UTF-8"));
			return new String(md.digest(), "UTF-8");
		}
		catch (Exception ex) {
			
		}
		return "";
	}
}
