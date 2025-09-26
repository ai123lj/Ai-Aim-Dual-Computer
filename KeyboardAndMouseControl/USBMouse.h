#ifndef USB_MOUSE_H
#define USB_MOUSE_H

#include "USB.h"
#include "USBHID.h"

#define MOUSE_LEFT     0x01
#define MOUSE_RIGHT    0x02
#define MOUSE_MIDDLE   0x04
#define MOUSE_BACKWARD 0x08
#define MOUSE_FORWARD  0x10
#define MOUSE_ALL      0x1F

typedef struct TU_ATTR_PACKED {
  uint8_t buttons; /**< buttons mask for currently pressed buttons in the mouse. */
  int16_t x;       /**< Current delta x movement of the mouse. */
  int16_t y;       /**< Current delta y movement on the mouse. */
  int8_t wheel;    /**< Current delta wheel movement on the mouse. */
  int8_t pan;      // using AC Pan
} hid_mouseL_report_t;

class CustomHIDDevice : public USBHIDDevice {
public:
  CustomHIDDevice(void);
  void begin(void);
  bool sendKeyboardReport(uint8_t modifiers, uint8_t *keys);
  bool sendMouseReport(uint8_t buttons, int16_t x, int16_t y, int8_t wheel);

protected:
  uint16_t _onGetDescriptor(uint8_t *buffer) override;

private:
  static const uint8_t report_descriptor[];
};

#endif