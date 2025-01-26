It's a small unity package, which give you ability for control windows in your application

You can use Screen as base type for create your own window type
Example:
    public class InventoryScreen : Screen
    {
        [SerializeField] private Transform inventoryGrid;
        [SerializeField] private InventoryMoneyView moneyView;
        private InventorySlotView[] _itemSlots;
        private Core.Character.Inventory.Inventory _inventory;
        
        [Inject] 
        private void Setup(Core.Character.Inventory.Inventory inventory)
        {
            _inventory = inventory;
        }
        
        public override void OnOpened(IScreenOptions options)
        {
            base.OnOpened(options);
            _itemSlots = inventoryGrid.GetComponentsInChildren<InventorySlotView>();
            foreach (var item in _inventory.GetItemList())
            {
                if (item.Item.IsNotSlotItem)
                {
                    moneyView.SetText(item.Count.ToString());
                    continue;
                }
                _itemSlots[item.SlotId].SetItem(item);
            }
        }
    }
In inspector you can use checkbox IsReusableScreen for describes YourScreen as reusable screen
Reusable screen can be hided without destroying and will be showed with previous data in future
    
Now you need add this script as component in root object of window prefab
You can then manage this window using the service
First you need create serivce, for example in di installer:
Container.BindInstance(new ScreenService(screenContainer)).AsSingle().NonLazy();
As argument you need to use some transform which will parent for all windows
When you use Show() method, window prefab will be instantiated as child of this transform.
You can use next methods of ScreenService:

Show<YourScreen>(ISceenOptions options) - Instantiates or shows previously disabled prefab with YourScreen component in the specifed root transform
You can use ScreenOptions class for transfer data to YourScreen component
Example:
int a = 5;
Show<YourScreen>(new ScreenOptions<int>(a)); - send data to YourScreen component

In YourScreen class:
public override void OnOpened(IScreenOptions options) - recive data 
{
    base.OnOpened(options);
    if (options.Value is not int data) return; - check if data correct and cast object to your data type
    Debug.Log(data); - use data
}


if prefab with Component YourSceen doesn't exist, it throws exeption
if prefab was instantiated previously and now enabled nothing will happen

Close<YourScreen>() - Destroys prefab with YourScreen component in the specifed root transform
if prefab with Component didnt't instantiated it throws exception

Hide<YourScreen>() - Disables prefab with YourScreen component in the specifed root transform
if prefab with Component didnt't instantiated it throws exception
if YourScreen prefab is not ReusableScreen it will be work as Close<YourScreen>()

You can use some events in YourScreen script for recive data or control lifecycle of your objects, you need override it for use:
YourScreen.OnOpened(IScreenOptions options) - executes every time when prefab with YourScreen component instatiates 
options is null by default
YourScreen.OnDisplay() - executes every time when prefab with YourScreen component instatiates, and when hided prefabs shows again
YourScreen.OnClosed() - executes every time when prefab with YourScreen component destroys
YourScreen.OnHide() - executes every time when prefab with YourScreen component destroys and when reusableView hides 
