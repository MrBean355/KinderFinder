package com.dvt.kinderfinder.transmitter;

import java.util.LinkedList;

import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.Toast;

public class MainActivity extends ActionBarActivity {
	private static final String	SERVER_ADDRESS	= "http://192.168.1.7:55555/";

	private Spinner				restaurantsSpinner;
	private EditText			transmitterIdBox;
	private Button				selectButton;
	private ProgressBar			progressBar;
	private boolean				imDone			= false;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		restaurantsSpinner = (Spinner) findViewById(R.id.RestaurantList);
		transmitterIdBox = (EditText) findViewById(R.id.TransmitterId);
		selectButton = (Button) findViewById(R.id.SelectRestaurant);
		progressBar = (ProgressBar) findViewById(R.id.ProgressBar);

		load();
	}

	@Override
	protected void onDestroy() {
		super.onDestroy();

		JsonBuilder jb = new JsonBuilder();
		jb.addEntry("ID", transmitterIdBox.getText().toString());
		new RequestTask().execute(SERVER_ADDRESS + "api/releaseid", jb
				.toString());
	}

	private void load() {
		selectButton.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View arg0) {
				String restaurant = restaurantsSpinner.getSelectedItem()
						.toString();

				if (restaurant.equals("")) {
					Toast.makeText(getApplicationContext(),
							"No restaurant selected", Toast.LENGTH_SHORT)
							.show();
					return;
				}

				// TODO: Tell server the restaurant we chose.
			}
		});

		// Request an ID:
		new RequestTask() {
			@Override
			protected void onPostExecute(String result) {
				transmitterIdBox.setText(result);

				if (imDone) {
					progressBar.setVisibility(View.GONE);
				}
				else {
					imDone = true;
				}
			}
		}.execute(SERVER_ADDRESS + "api/getid");

		// Request restaurant list:
		new RequestTask() {
			@Override
			protected void onPostExecute(String result) {
				LinkedList<String> restaurants = JsonBuilder.JsonToList(result);
				restaurantsSpinner.setAdapter(new ArrayAdapter<>(
						getApplicationContext(),
						android.R.layout.simple_spinner_item, restaurants));

				if (imDone) {
					progressBar.setVisibility(View.GONE);
				}
				else {
					imDone = true;
				}
			}
		}.execute(SERVER_ADDRESS + "api/restaurantlist");
	}
}
