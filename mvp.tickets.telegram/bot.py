import asyncio
import logging
from aiogram import Bot, Dispatcher, types, Router, F
from aiogram.utils.keyboard import InlineKeyboardBuilder, ReplyKeyboardBuilder
from aiogram.fsm.storage.memory import MemoryStorage
from aiogram.fsm.context import FSMContext
from aiogram.fsm.state import StatesGroup, State
import http.client
import json
import ssl
from config_reader import config

ssl._create_default_https_context = ssl._create_unverified_context

logging.basicConfig(level=logging.INFO)

class CreateTicket(StatesGroup):
    save_phone_number = State()
    save_name = State()
    save_text = State()
    add_attachments = State()
    list = State()

mainButtons = InlineKeyboardBuilder()
mainButtons.add(types.InlineKeyboardButton(
    text="Создать заявку",
    callback_data="create_ticket")
)
mainButtons.add(types.InlineKeyboardButton(
    text="Посмотреть последние заявки",
    callback_data="list_tickets")
)

cancelButton = InlineKeyboardBuilder()
cancelButton.add(types.InlineKeyboardButton(
    text="Вернуться в главное меню",
    callback_data="cancel")
)

router = Router()

@router.callback_query(F.data == "cancel")
async def cancel(callback: types.CallbackQuery, state: FSMContext):
    await callback.message.answer("Выберите действие", reply_markup=mainButtons.as_markup())
    await state.clear()

@router.callback_query(F.data == "create_ticket")
async def create_ticket(callback: types.CallbackQuery, state: FSMContext):
    builder = ReplyKeyboardBuilder()
    builder.row(
        types.KeyboardButton(text="Предоставить номер телефона", request_contact=True)
    )
    await callback.message.answer("Требуется номер телефона", reply_markup=builder.as_markup(resize_keyboard=True, one_time_keyboard=True))
    await state.set_state(CreateTicket.save_phone_number)

@router.callback_query(F.data == "list_tickets")
async def list_tickets(callback: types.CallbackQuery, state: FSMContext):
    builder = ReplyKeyboardBuilder()
    builder.row(
        types.KeyboardButton(text="Предоставить номер телефона", request_contact=True)
    )
    await callback.message.answer("Требуется номер телефона", reply_markup=builder.as_markup(resize_keyboard=True, one_time_keyboard=True))
    await state.set_state(CreateTicket.list)

@router.callback_query(F.data == "confirm_submit")
async def confirm_submit(callback: types.CallbackQuery):
    buttons = InlineKeyboardBuilder()
    buttons.add(types.InlineKeyboardButton(
        text="Да",
        callback_data="submit")
    )
    buttons.add(types.InlineKeyboardButton(
        text="Вернуться в главное меню",
        callback_data="cancel")
    )
    await callback.message.answer("Создать заявку?", reply_markup=buttons.as_markup())

@router.callback_query(F.data == "add_attachments")
async def add_attachments(callback: types.CallbackQuery, state: FSMContext):
    buttons = InlineKeyboardBuilder()
    buttons.add(types.InlineKeyboardButton(
        text="Вернуться в главное меню",
        callback_data="cancel")
    )
    await callback.message.answer("Вы можите прекрепить до 5 файлов:", reply_markup=buttons.as_markup())
    await state.set_state(CreateTicket.add_attachments)

@router.callback_query(F.data == "submit")
async def submit(callback: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    conn = http.client.HTTPSConnection(config.api.get_secret_value())
    headers = {'Content-type': 'application/json'}
    payload = {
        'ApiKey':config.api_key.get_secret_value(),
        'Phone': user_data['phone'],
        'firstName': callback.from_user.first_name,
        'lastName': callback.from_user.last_name,
        'Name': user_data['name'],
        'Text': user_data['text'],
        'Files': user_data['files'] if 'files' in user_data else [],
    }
    json_data = json.dumps(payload)
    conn.request('POST', '/api/tickets/telegram', json_data, headers)
    response = conn.getresponse()
    responseData = json.loads(response.read().decode())
    await callback.message.answer("<a href='"+responseData['link']+"'>Ссылка на созданную заявку "+responseData['link']+"</a>", reply_markup=cancelButton.as_markup(), parse_mode="HTML")
    await state.clear()

@router.message(CreateTicket.list)
async def list(message: types.Message, state: FSMContext):
    if message.contact is None or message.contact.user_id != message.from_user.id:
        await message.answer("Выберите действие", reply_markup=mainButtons.as_markup())
        await state.clear()
    else:
        conn = http.client.HTTPSConnection(config.api.get_secret_value())
        headers = {'Content-type': 'application/json'}
        payload = {
            'ApiKey':config.api_key.get_secret_value(),
            'Phone': message.contact.phone_number
        }
        json_data = json.dumps(payload)
        conn.request('POST', '/api/tickets/telegram/list', json_data, headers)
        response = conn.getresponse()
        responseData = json.loads(response.read().decode())
        html = ""
        for ticket in responseData['data']:
            html += "<a href='"+ticket['link']+"'>Название: "+ticket['name']+", дата создания: "+ticket['dateCreated']+", ссылка: "+ticket['link']+"</a>\n"
            html += "------------------------\n"
        logging.info(html)
        await message.answer(html, reply_markup=cancelButton.as_markup(), parse_mode='HTML')
        await state.clear()

@router.message(CreateTicket.save_phone_number)
async def save_phone_number(message: types.Message, state: FSMContext):
    if message.contact is None or message.contact.user_id != message.from_user.id:
        await message.answer("Выберите действие", reply_markup=mainButtons.as_markup())
        await state.clear()
    else:
        await state.update_data(phone=message.contact.phone_number)
        await message.answer("Введите название заявки:", reply_markup=cancelButton.as_markup())
        await state.set_state(CreateTicket.save_name)

@router.message(CreateTicket.save_name)
async def save_name(message: types.Message, state: FSMContext):
    await state.update_data(name=message.text)
    await message.answer("Введите описание заявки:", reply_markup=cancelButton.as_markup())
    await state.set_state(CreateTicket.save_text)

@router.message(CreateTicket.save_text)
async def save_text(message: types.Message, state: FSMContext):
    await state.update_data(text=message.text)
    buttons = InlineKeyboardBuilder()
    buttons.add(types.InlineKeyboardButton(
        text="Да",
        callback_data="add_attachments")
    )
    buttons.add(types.InlineKeyboardButton(
        text="Нет",
        callback_data="confirm_submit")
    )
    await message.answer("Прикрепить файлы?", reply_markup=buttons.as_markup())

@router.message(CreateTicket.add_attachments, F.content_type.in_({'photo', 'document'}))
async def doc_handler(message: types.Message, state: FSMContext):
    user_data = await state.get_data()
    files = user_data['files'] if 'files' in user_data else []
    if len(files) < 5:
        if message.photo:
            files.append(message.photo[-1].file_id)
        if message.document:
            files.append(message.document.file_id)
        await state.update_data(files=files)
    user_data = await state.get_data()
    buttons = InlineKeyboardBuilder()
    buttons.add(types.InlineKeyboardButton(
        text="Да",
        callback_data="submit")
    )
    buttons.add(types.InlineKeyboardButton(
        text="Вернуться в главное меню",
        callback_data="cancel")
    )
    await message.answer("Создать заявку?", reply_markup=buttons.as_markup())

@router.message()
async def actions(message: types.Message, state: FSMContext):
    await state.clear()
    await message.answer("Выберите действие", reply_markup=mainButtons.as_markup())

async def main():
    bot = Bot(token=config.bot_token.get_secret_value())
    dp = Dispatcher(storage=MemoryStorage())
    dp.include_routers(router)
    await bot.delete_webhook(drop_pending_updates=True)
    await dp.start_polling(bot)


if __name__ == "__main__":
    asyncio.run(main())