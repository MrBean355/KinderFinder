package com.dvt.kinderfinder.transmitter;

import java.io.ByteArrayOutputStream;
import java.io.IOException;

import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.StatusLine;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.impl.client.DefaultHttpClient;

import android.support.v7.app.ActionBarActivity;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.View;
import android.widget.*;

public class MainActivity extends ActionBarActivity {
	private static final String	SERVER_ADDRESS	= "http://192.168.1.7:55555/";
	private EditText			transmitterIdBox;
	private Button				requestButton;
	private ProgressBar			progressBar;

	// Params, Progress, Result
	class RequestTask extends AsyncTask<String, String, String> {

		@Override
		protected String doInBackground(String... uri) {
			HttpClient httpclient = new DefaultHttpClient();
			HttpResponse response;
			String responseString = null;

			try {
				response = httpclient.execute(new HttpPost(uri[0]));
				StatusLine statusLine = response.getStatusLine();

				if (statusLine.getStatusCode() == HttpStatus.SC_OK) {
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
			catch (Exception ex) {
				System.out.println("Error: " + ex);
				Toast.makeText(getApplicationContext(),
						"Error contacting server", Toast.LENGTH_LONG).show();
			}

			return responseString;
		}

		@Override
		protected void onPostExecute(String result) {
			super.onPostExecute(result);
			transmitterIdBox.setText(Utility.removeQuotations(result));
			progressBar.setVisibility(View.GONE);
		}
	}

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		transmitterIdBox = (EditText) findViewById(R.id.TransmitterId);
		requestButton = (Button) findViewById(R.id.Request);
		progressBar = (ProgressBar) findViewById(R.id.ProgressBar);

		progressBar.setVisibility(View.GONE);

		requestButton.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View arg0) {
				progressBar.setVisibility(View.VISIBLE);
				new RequestTask().execute(SERVER_ADDRESS + "api/ping");
			}
		});
	}
}
