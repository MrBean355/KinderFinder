package com.dvt.kinderfinder.transmitter;

import org.apache.http.HttpStatus;

import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;

public class MainActivity extends ActionBarActivity {
	private EditText emailBox;
	private EditText passwordBox;
	private Button logInButton;
	private ProgressBar progressBar;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		emailBox = (EditText) findViewById(R.id.emailBox);
		passwordBox = (EditText) findViewById(R.id.passwordBox);
		logInButton = (Button) findViewById(R.id.logInButton);
		progressBar = (ProgressBar) findViewById(R.id.progressBar);
		
		logInButton.setOnClickListener(logInButtonHandler);
		progressBar.setVisibility(View.GONE);
	}
	
	private OnClickListener logInButtonHandler = new OnClickListener() {
		@Override // hi
		public void onClick(View v) {
			String email = emailBox.getText().toString();
			String password = passwordBox.getText().toString();
			String hashed = Utility.hashPassword(password);
			System.out.println("Hash: " + hashed);
			
			/*JsonBuilder jb = new JsonBuilder();
			jb.addEntry("EmailAddress", email);
			jb.addEntry("PasswordHash", password);
			
			logInButton.setEnabled(false);
			progressBar.setVisibility(View.VISIBLE);
			
			new RequestTask(){
				@Override
				protected void onPostExecute(String result) {
					if (statusCode == HttpStatus.SC_OK) {
						// TODO: Start new activity.
						finish();
					}
					else {
						logInButton.setEnabled(true);
						progressBar.setVisibility(View.GONE);
					}
				}
			}.execute("api/login", jb.toString());*/
		}
	};
}
