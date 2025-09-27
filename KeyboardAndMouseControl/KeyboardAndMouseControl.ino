#include "USB.h"
#include "USBHIDMouse.h"
#include "USBHIDKeyboard.h"
USBHIDMouse Mouse;
USBHIDKeyboard Keyboard;

void setup() {
  Serial.begin(2000000);

  Mouse.begin();
  Keyboard.begin();
  USB.begin();
}
char buf[50];
void loop() {
  // use serial input to control the mouse:
  if (Serial.available() > 0) {
    //验证头
    if (Serial.read() != '{')
      return;
    //头正确，开始接收
    delayMicroseconds(300);
    int len;
    for (len = 0; Serial.available() > 0; len++) {
      buf[len] = Serial.read();
    }
    //验证尾
    if (buf[len - 1] != '}')
      return;
    buf[len - 1] = 0;

    int mode = 0, x = 0, y = 0;
    sscanf(buf, "%d,%d,%d", &mode, &x, &y);
    switch (mode) {
      case 0:
        moveMouse(x, y, 0);  //只移动鼠标
        break;
      case 1:
        moveMouse(x, y, MOUSE_LEFT);  //移动鼠标并射击
        break;
      case 2:
        Mouse.press(MOUSE_LEFT);
        break;
      case 3:
        Mouse.release(MOUSE_LEFT);
        break;
      case 4:
        moveMouse(x, y, MOUSE_LEFT);  //移动鼠标并射击
        delay(35);
        Keyboard.press('2');  //自动切枪
        delay(35);
        Keyboard.release('2');
        break;
      case 5:
        moveMouse(x, y, MOUSE_LEFT);  //移动鼠标并射击
        delay(30);
        Keyboard.press('3');  //自动切枪
        delay(30);
        Keyboard.release('3');
        break;
    }
    Serial.print("{ok}");

    // moveMouse(x, y);
    // switch (mode) {
    //   case 1:
    //     Mouse.click(MOUSE_LEFT);
    //     break;
    //   case 2:
    //     Mouse.press(MOUSE_LEFT);
    //     break;
    //   case 3:
    //     Mouse.release(MOUSE_LEFT);
    //     break;
    //   case 4:
    //     Mouse.click(MOUSE_LEFT);
    //     delay(30);
    //     Keyboard.press('2');
    //     delay(30);
    //     Keyboard.release('2');
    //     break;
    // }
  }
}
void moveMouse(int x, int y, int button) {
  int num;
  if (abs(x) > abs(y)) {
    num = abs(x);
  } else {
    num = abs(y);
  }

  hid_mouse_report_t report;
  if (num <= 120) {  //一次移动加点击
    report = { .buttons = button, .x = x, .y = y, .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
    report = { .buttons = 0, .x = 0, .y = 0, .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
  } else if (num <= 240) {
    report = { .buttons = 0, .x = x / 2, .y = y / 2, .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
    report = { .buttons = button, .x = x / 2 + (x % 2), .y = y / 2 + (y % 2), .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
    report = { .buttons = 0, .x = 0, .y = 0, .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
  } else if (num <= 360) {
    report = { .buttons = 0, .x = x / 3, .y = y / 3, .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
    Mouse.sendReport(report);
    report = { .buttons = button, .x = x / 3 + (x % 3), .y = y / 3 + (y % 3), .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
    report = { .buttons = 0, .x = 0, .y = 0, .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
  } else if (num <= 480) {
    report = { .buttons = 0, .x = x / 4, .y = y / 4, .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
    Mouse.sendReport(report);
    Mouse.sendReport(report);
    report = { .buttons = button, .x = x / 4 + (x % 4), .y = y / 4 + (y % 4), .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
    report = { .buttons = 0, .x = 0, .y = 0, .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
  } else if (num <= 600) {
    report = { .buttons = 0, .x = x / 5, .y = y / 5, .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
    Mouse.sendReport(report);
    Mouse.sendReport(report);
    Mouse.sendReport(report);
    report = { .buttons = button, .x = x / 5 + (x % 5), .y = y / 5 + (y % 5), .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
    report = { .buttons = 0, .x = 0, .y = 0, .wheel = 0, .pan = 0 };
    Mouse.sendReport(report);
  }
}
