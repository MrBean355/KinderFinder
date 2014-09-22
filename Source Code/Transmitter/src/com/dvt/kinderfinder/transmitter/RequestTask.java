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
	protected int statusCode;
	
	@Override
	protected String doInBackground(String... data) {
		if (data.length == 0) {
			System.out.println("[Error] Tried to send data without enough arugments.");
			return "";
		}

		HttpClient httpclient = new DefaultHttpClient();
		HttpPost httppost = new HttpPost(data[0]);
		httppost.addHeader("content-type", "application/json");
		HttpResponse response;
		String responseString = null;

		try {
			// Check whether any data is being sent:
			if (data.length > 1 && !data[1].equals("")) {
				httppost.setEntity(new StringEntity(data[1]));
			}

			response = httpclient.execute(httppost);
			StatusLine statusLine = response.getStatusLine();
			statusCode = statusLine.getStatusCode();

			if (statusCode == HttpStatus.SC_OK) {
				ByteArrayOutputStream out = new ByteArrayOutputStream();
				response.getEntity().writeTo(out);
				out.close();
				responseString = out.toString();
			}
			else {
				response.getEntity().getContent().close();
				throw new IOException(statusLine.getReasonPhrase());
			}
		}
		catch (Exception e) {
			System.out.println("Error while sending data: " + e);
		}

		return responseString;
	}
}
