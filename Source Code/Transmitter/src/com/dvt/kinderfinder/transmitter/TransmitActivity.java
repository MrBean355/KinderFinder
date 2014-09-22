package com.dvt.kinderfinder.transmitter;

import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.widget.Toast;

public class TransmitActivity extends ActionBarActivity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_transmit);
		
		Toast.makeText(getApplicationContext(), "Welcome " + getIntent().getStringExtra("ID"), Toast.LENGTH_LONG).show();
	}
}
