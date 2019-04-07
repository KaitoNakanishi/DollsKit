import discord
import asyncio
import concurrent.futures
import sys
import traceback

class MyClient(discord.Client):
	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)

		# create the background task and run it in the background
		self.bg_task = self.loop.create_task(self.stdin_task())

	async def cmd_ch(self):
		for ch in self.get_all_channels():
			print(ch.id, ch.name)

	async def cmd_msg(self, args):
		if len(args) < 2:
			print('msg <ch_id> <msg>')
			return
		id, msg = args
		ch = discord.utils.get(self.get_all_channels(), id=id)
		if not ch:
			print('Channel not found:', id)
			return
		await self.send_message(ch, msg)

	async def stdin_task(self):
		try:
			print('stdin task start')
			with concurrent.futures.ThreadPoolExecutor(max_workers=1) as pool:
				while True:
					await self.wait_until_ready()
					await asyncio.sleep(0.1)
					line = await self.loop.run_in_executor(pool, input, "Cmd: ")
					sp = line.split()
					if len(sp) < 1:
						continue
					cmd, *args = sp
					if cmd == 'ch':
						await self.cmd_ch()
					elif cmd == 'msg':
						await self.cmd_msg(args)
					elif cmd == 'exit':
						await self.logout()
						break
					else:
						print('Unknown command')
		except Exception:
			traceback.print_exc()

	async def on_ready(self):
		print('Logged in as')
		print(self.user.name)
		print(self.user.id)
		print('------')

client = MyClient()
#client.run('token')
