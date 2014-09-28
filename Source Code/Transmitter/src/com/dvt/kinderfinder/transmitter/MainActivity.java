package com.dvt.kinderfinder.transmitter;

import java.util.LinkedList;

import org.apache.http.HttpStatus;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.Toast;

public class MainActivity extends ActionBarActivity {
	private Spinner restaurantList;
	private Spinner typeList;
	private Button transmitButton;
	private EditText xPosBox, yPosBox;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		restaurantList = (Spinner) findViewById(R.id.restaurantList);
		typeList = (Spinner) findViewById(R.id.typeList);
		xPosBox = (EditText) findViewById(R.id.xPosBox);
		yPosBox = (EditText) findViewById(R.id.yPosBox);
		transmitButton = (Button) findViewById(R.id.transmitButton);
		
		restaurantList.setOnItemSelectedListener(spinnerItemSelectedhandler);
		transmitButton.setOnClickListener(buttonClickHandler);
		
		xPosBox.clearFocus();
		loadRestaurants();
	}
	
	/**
	 * When an item in the restaurant list is selected, loads the list of tags
	 * that belong to the restaurant.
	 */
	private OnItemSelectedListener spinnerItemSelectedhandler = new OnItemSelectedListener() {

		@Override
		public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
			loadTypes(restaurantList.getSelectedItem().toString());
		}

		@Override
		public void onNothingSelected(AdapterView<?> parent) { }
	};
	
	private OnClickListener buttonClickHandler = new OnClickListener() {
		@Override
		public void onClick(View v) {
			String restaurant = restaurantList.getSelectedItem().toString();
			String type = typeList.getSelectedItem().toString();
			String x = xPosBox.getText().toString();
			String y = yPosBox.getText().toString();
			
			if (restaurant.equals("") || type.equals("")) {
				Toast.makeText(getApplicationContext(), "Something went wrong; please try again", Toast.LENGTH_SHORT).show();
				return;
			}
			else if (x.equals("") || y.equals("")) {
				Toast.makeText(getApplicationContext(), "Please enter x and y co-ords", Toast.LENGTH_SHORT).show();
				return;
			}
			
			JsonBuilder jb = new JsonBuilder();
			jb.addEntry("RestaurantName", restaurant);
			jb.addEntry("TransmitterType", type);
			jb.addEntry("X", x);
			jb.addEntry("Y", y);
			
			new RequestTask() {
				@Override
				protected void onPostExecute(String result) {
					if (statusCode == HttpStatus.SC_OK) {
						Intent intent = new Intent(getApplicationContext(), TransmitActivity.class);
						intent.putExtra("ID", result);
						startActivity(intent);
						finish();
					}
					else {
						Toast.makeText(getApplicationContext(), "An error occured; please try again", Toast.LENGTH_SHORT).show();
					}
				}
			}.execute("api/assignrestaurant", jb.toString());
		}
	};
	
	/**
	 * Requests a list of all restaurants from the server.
	 */
	private void loadRestaurants() {
		restaurantList.setEnabled(false);
		typeList.setEnabled(false);
		xPosBox.setEnabled(false);
		yPosBox.setEnabled(false);
		transmitButton.setEnabled(false);
		
		new RequestTask() {
			@Override
			protected void onPostExecute(String result) {
				if (statusCode == HttpStatus.SC_OK) {
					LinkedList<String> restaurants = JsonBuilder.JsonToList(result);
					ArrayAdapter<String> adapter = new ArrayAdapter<>(getApplicationContext(), R.layout.spinner_item, restaurants);
					adapter.setDropDownViewResource(R.layout.spinner_dropdown_item);
					restaurantList.setAdapter(adapter);
					
					if (restaurants.size() > 0) {
						restaurantList.setEnabled(true);
					}
				}
				else {
					Toast.makeText(getApplicationContext(), "Error contacting server", Toast.LENGTH_SHORT).show();
				}
			}
		}.execute("api/restaurantlist");
	}
	
	/**
	 * Requests a list of tags that belong to a specific restaurant from the
	 * server.
	 */
	private void loadTypes(String restaurant) {
		JsonBuilder jb = new JsonBuilder();
		jb.addEntry("RestaurantName", restaurant);
		
		new RequestTask() {
			@Override
			protected void onPostExecute(String result) {
				if (statusCode == HttpStatus.SC_OK) {
					LinkedList<String> types = JsonBuilder.JsonToList(result);
					ArrayAdapter<String> adapter = new ArrayAdapter<>(getApplicationContext(), R.layout.spinner_item, types);
					adapter.setDropDownViewResource(R.layout.spinner_dropdown_item);
					typeList.setAdapter(adapter);
					
					if (types.size() > 0) {
						typeList.setEnabled(true);
						xPosBox.setEnabled(true);
						yPosBox.setEnabled(true);
						transmitButton.setEnabled(true);
					}
					else {
						Toast.makeText(getApplicationContext(), "This restaurant has no more available types", Toast.LENGTH_LONG).show();
					}
				}
				else {
					Toast.makeText(getApplicationContext(), "Error contacting server", Toast.LENGTH_SHORT).show();
				}
			}
		}.execute("api/transmittertype", jb.toString());
	}
}
