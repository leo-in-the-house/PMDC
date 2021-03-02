import os
import httplib2
import xml.etree.ElementTree as ET
import glob
import time
from apiclient import discovery

SHEET_CONTENT_START = 2
WAIT_TIME = 3

class ItemGen:
    """
    A class for syncronizing the strings in the project files
    with the google doc spreadsheets containing translations.
    """
    def open_sheet(self, credentials, id):
        http = credentials.authorize(httplib2.Http())
        discoveryUrl = ('https://sheets.googleapis.com/$discovery/rest?' 'version=v4')
        self._service = discovery.build('sheets', 'v4', http=http, discoveryServiceUrl=discoveryUrl)
        self._id = id

    def close_sheet(self):
        pass


    def write_data_text(self, txt_name, sheet_name):
        """
        Writes data from a txt to the sheet
        """
        header, table = self._query_txt(os.path.join("..", "temp", txt_name+".txt"))
        # merge the data together, back into the google sheets
        self._write_sheet_table(sheet_name, table)

    def load_sheet_text(self, txt_name, sheet_name):
        """
        Loads sheet text and writes it to txt file.
        """
        header_row, sheet = self._query_sheet(sheet_name)

        self._write_txt(os.path.join("..", "temp", txt_name + ".out.txt"), header_row, sheet)

        """
        cur_index = 900

        item_sheet = []
        trade_sheet = []
        current_trade_chain = []
        rarity_sheet = []
        item_header = ["Index"] + header_row[:5] + header_row[9:]
        #exclusive items
        for row in sheet:
            effect = int(row[10][3:6])
            if effect > 0:
                item_sheet.append([str(cur_index)] + row[:5] + row[9:])

                if row[7] == 'A':
                    for trade_row, trade_idx in enumerate(current_trade_chain):
                        trade_sheet.append([trade_row[0], "-1"])
                    current_trade_chain = []
                else:
                    current_trade_chain.append([str(cur_index), row[8]])

                rarity = int(row[9][0:1])
                rarity_sheet.append([str(cur_index), rarity])
            cur_index = cur_index + 1

        if len(current_trade_chain) > 0:
            for trade_row, trade_idx in enumerate(current_trade_chain):
                trade_sheet.append([trade_row[0], "-1"])
            current_trade_chain = []

        self._write_txt(os.path.join("..", "temp", txt_name + ".out.txt"), item_header, item_sheet)

        #species to item xml



        data_string_info_path = os.path.join("..", "Asset", "Data", "Script", "common_gen.lua")
        with open(data_string_info_path, 'w', encoding='utf-8') as txt:
            txt.write("--[[" + \
                "    common_gen.lua" + \
                "    A generated collection of values" + \
                "]]--" + \
                "COMMON_GEN = {}" + \
                "" + \
                "COMMON_GEN.TRADES = {")

            # trade list
            for row in trade_sheet:
                trade_item = row[0]
                req_items = row[1]
                txt.write("{ Item="+trade_item+", ReqItem={"+"".join(req_items)+"}}")

            txt.write("}\n")

            txt.write("COMMON_GEN.TRADES_RANDOM = {")
            # random trade list
            for row in rarity_sheet:
                trade_item = row[0]
                req_items = ["-1"] * (row[1] + 1)
                txt.write("{ Item="+trade_item+", ReqItem={"+"".join(req_items)+"}}")

            txt.write("}\n")

        """




    def _query_sheet(self, sheet_name):
        """
        Gets all data from a given google sheet, automatically figuring out height/width.
        Includes header row.
        """
        check_range = sheet_name + "!A"+str(SHEET_CONTENT_START)+":A"
        check_result = self._service.spreadsheets().values().get(spreadsheetId=self._id, range=check_range).execute()
        total_rows = len(check_result.get('values', []))

        return self._query_sheet_range(sheet_name, sheet_name + "!"+str(SHEET_CONTENT_START)+":"+str(SHEET_CONTENT_START+total_rows-1))

    def _query_sheet_range(self, sheet_name, range_name):
        """
        Gets all values from a given google sheet, automatically figuring out height/width.
        Includes header row.
        """

        header_range = sheet_name + "!A"+str(SHEET_CONTENT_START-1)+":"+str(SHEET_CONTENT_START-1)
        header_result = self._service.spreadsheets().values().get(spreadsheetId=self._id, range=header_range).execute()
        header_row = header_result.get('values', [[]])[0]

        result = self._service.spreadsheets().get(spreadsheetId=self._id, ranges=range_name, includeGridData=True).execute()
        time.sleep(WAIT_TIME)

        content_rows = []
        for row in result['sheets'][0]['data'][0]['rowData']:
            content_row = []
            for cell in row['values']:
                if 'userEnteredValue' in cell and 'stringValue' in cell['userEnteredValue']:
                    content_str = cell['userEnteredValue']['stringValue']
                elif 'userEnteredValue' in cell and 'boolValue' in cell['userEnteredValue']:
                    content_str = str(cell['userEnteredValue']['boolValue'])
                elif 'userEnteredValue' in cell and 'numberValue' in cell['userEnteredValue']:
                    content_str = str(cell['userEnteredValue']['numberValue'])
                else:
                    content_str = ''
                if '\n' in content_str:
                    print('Newline found in ' + sheet_name + ': ' + content_str)
                content_row.append(content_str)

            content_rows.append(content_row)


        return header_row, content_rows

    def _query_txt(self, txt_path):
        """
        Gets all values from a pre-formatted txt file, separated by header and values.
        """
        header = []
        output = []
        in_header = True
        with open(txt_path, encoding='utf-8') as txt:
            for line in txt:
                # remember to remove the newline
                cols = line[:-1].split('\t')
                if not in_header:
                    output.append(cols)
                else:
                    header = cols
                in_header = False
        return header, output


    def _write_sheet_table(self, sheet_name, table):
        """
        Takes the local array of values,
        and writes directly to the remote google sheet.
        """

        range_name = sheet_name + "!" + str(SHEET_CONTENT_START) + ":" + str(SHEET_CONTENT_START+len(table)-1)
        body = {'values': table}
        self._service.spreadsheets().values().update(spreadsheetId=self._id, range=range_name,
                                                     valueInputOption="RAW", body=body).execute()




    def _write_txt(self, txt_path, header_row, sheet):
        """
        Writes all translation entries to a pre-formatted txt file.
        """
        with open(txt_path, 'w', encoding='utf-8') as txt:
            txt.write('\t'.join(header_row)+"\n")
            for line in sheet:
                txt.write('\t'.join(line)+"\n")

