package com.dvt.kinderfinder.transmitter;

import java.io.ByteArrayOutputStream;
import java.io.IOException;

import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.StatusLine;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;

import android.os.AsyncTask;

class RequestTask extends AsyncTask<String, String, String> {
	private static final String SERVER_ADDRESS = "http://192.168.1.7:55555/";
	protected int statusCode;
	
	@Override
	protected String doInBackground(String... data) {
		if (data.length == 0) {
			System.out.println("[Error] Tried to send data without enough arugments.");
			return "";
		}
		
		HttpClient client = new DefaultHttpClient();
		HttpPost post = new HttpPost(SERVER_ADDRESS + data[0]);
		post.addHeader("content-type", "application/json");
		HttpResponse response;
		
		try {
			// Check whether any data is being sent:
			if (data.length > 1 && !data[1].equals("")) {
				System.out.println("Sending: " + data[1]);
				post.setEntity(new StringEntity(data[1]));
			}

			response = client.execute(post);
			StatusLine statusLine = response.getStatusLine();
			statusCode = statusLine.getStatusCode();

			if (statusCode == HttpStatus.SC_OK) {
				ByteArrayOutputStream out = new ByteArrayOutputStream();
				response.getEntity().writeTo(out);
				out.close();
				return out.toString();
			}
			else {
				response.getEntity().getContent().close();
				throw new IOException(statusLine.getReasonPhrase());
			}
		}
		catch (Exception e) {
			System.out.println("Error while sending data to '" + SERVER_ADDRESS + data[0] + "': " + e);
		}
		finally {
			client.getConnectionManager().shutdown();
		}
		
		return "";
	}
}
