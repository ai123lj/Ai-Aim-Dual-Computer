#include "USBMouse.h"
#include "arduino.h"
USBHID HID;
const uint8_t CustomHIDDevice::report_descriptor[] = {
  0x05, 0x01,
  0x09, 0x02,
  0xA1, 0x01,

  0x85, 0x02,

  0x09, 0x01,
  0xA1, 0x00,
  // Buttons (5 buttons)
  0x05, 0x09,
  0x19, 0x01,
  0x29, 0x05,
  0x15, 0x00,
  0x25, 0x01,
  0x95, 0x05,
  0x75, 0x01,
  0x81, 0x02,
  // Padding (3 bits)
  0x95, 0x01,
  0x75, 0x03,
  0x81, 0x01,
  // X/Y (8-bit relative)
  0x05, 0x01,
  0x09, 0x30,
  0x09, 0x31,
  0x16, 0x01,0x81,
  0x26, 0xff,0x7F,
  0x95, 0x02,
  0x75, 0x10,
  0x81, 0x06,
  // Vertical Wheel (8-bit relative)
  0x09, 0x38,
  0x15, 0x81,
  0x25, 0x7F,
  0x95, 0x01,
  0x75, 0x08,
  0x81, 0x06,
  // Horizontal Wheel (8-bit relative)
  0x05, 0x0C,
  0x0A, 0x38, 0x02,
  0x15, 0x81,
  0x25, 0x7F,
  0x95, 0x01,
  0x75, 0x08,
  0x81, 0x06,
  0xC0,
  0xC0
};

CustomHIDDevice::CustomHIDDevice(void) {
  static bool initialized = false;
  if (!initialized) {
    initialized = true;
    HID.addDevice(this, sizeof(report_descriptor));
  }
}

void CustomHIDDevice::begin(void) {
  HID.begin();
}

uint16_t CustomHIDDevice::_onGetDescriptor(uint8_t *buffer) {
  memcpy(buffer, report_descriptor, sizeof(report_descriptor));
  return sizeof(report_descriptor);
}

bool CustomHIDDevice::sendKeyboardReport(uint8_t modifiers, uint8_t *keys) {
  uint8_t report[8] = { 0x01, modifiers, 0x00, keys[0], keys[1], keys[2], keys[3], keys[4] };
  return HID.SendReport(0, report, sizeof(report));
}

bool CustomHIDDevice::sendMouseReport(uint8_t buttons, int16_t x, int16_t y, int8_t wheel) {
  x = constrain(x, -32767, 32767);
  y = constrain(y, -32767, 32767);
  wheel = constrain(wheel, -127, 127);

  hid_mouseL_report_t report;
  report = { .buttons = buttons, .x = x, .y = y, .wheel = wheel, .pan = 0 };
  return HID.SendReport(HID_REPORT_ID_MOUSE, &report, sizeof(report));
}