package com.dvt.kinderfinder.transmitter;

import java.security.MessageDigest;

import android.util.Base64;

public class Utility {

	public static String hashPassword(String input) {
		try {
			String toHash = input + "2e6e76485b61254b2e73694d50";
			MessageDigest digest = MessageDigest.getInstance("SHA-256");
			byte[] hash = digest.digest(toHash.getBytes("UTF-8"));
			
			return Base64.encodeToString(hash, Base64.DEFAULT);
		}
		catch (Exception ex) {
			System.out.println("Error hashing password: " + ex);
		}

		return "";
	}
}
