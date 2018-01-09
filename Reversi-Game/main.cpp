/*
 * main.cpp
 *
 *  Created on: Oct 25, 2017
 *      Author: Ofir Ben-Shoham.
 *          ID: 208642496.
 */

#include <iostream>
#include "Board.h"
#include "Cell.h"
#include "GameLogic.h"
#include "CellsSwitcher.h"
#include <string>
#include "GameRunner.h"

using namespace std;
//sdsdsdsds
int main() {
	Board b;
	b.printBoard();

	try {
		GameRunner gr(b);
		gr.run();
	} catch (exception *e) {
		cout << "exception was catched :( \n";
	}

	return 0;
}

