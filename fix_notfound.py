import re

for f in ['src/LagerPro.Api/Controllers/ShippingController.cs',
          'src/LagerPro.Api/Controllers/InventoryController.cs']:
    with open(f) as fh:
        content = fh.read()
    content = content.replace('not found." }', 'ble ikke funnet." }')
    with open(f, 'w') as fh:
        fh.write(content)
    print(f"Fixed {f}")
